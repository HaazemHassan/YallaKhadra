using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using YallaKhadra.Core.Abstracts.InfrastructureAbstracts;
using YallaKhadra.Core.Abstracts.ServicesContracts;
using YallaKhadra.Core.Bases;
using YallaKhadra.Core.Bases.Authentication;
using YallaKhadra.Core.Entities.IdentityEntities;
using YallaKhadra.Core.Enums;

namespace YallaKhadra.Services.Services {
    public class AuthenticationService : IAuthenticationService {

        private readonly JwtSettings _jwtSettings;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IApplicationUserService _applicationUserService;
        private readonly GoogleAuthSettings _googleAuthSettings;
        private readonly IMapper _mapper;
        //private readonly IEmailService _emailService;



        public AuthenticationService(JwtSettings jwtSettings, UserManager<ApplicationUser> userManager, IRefreshTokenRepository refreshTokenRepository, IApplicationUserService applicationUserService, GoogleAuthSettings googleAuthSettings, IMapper mapper /*IEmailService emailService*/) {
            _jwtSettings = jwtSettings;
            _userManager = userManager;
            _refreshTokenRepository = refreshTokenRepository;
            _applicationUserService = applicationUserService;
            _googleAuthSettings = googleAuthSettings;
            _mapper = mapper;
            //_emailService = emailService;
        }


        public async Task<ServiceOperationResult<AuthResult?>> AuthenticateAsync(ApplicationUser user, DateTime? refreshTokenExpDate) {
            if (user is null)
                return ServiceOperationResult<AuthResult?>.Failure(ServiceOperationStatus.InvalidParameters, "User cannot be null");

            var jwtSecurityToken = await GenerateAccessToken(user);
            var refreshToken = GenerateRefreshToken(user.Id, refreshTokenExpDate);
            await AddRefreshTokenToDatabase(refreshToken, jwtSecurityToken.Id);

            AuthResult jwtResult = new AuthResult {
                AccessToken = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                RefreshToken = refreshToken
            };
            return ServiceOperationResult<AuthResult?>.Success(jwtResult);
        }

        public async Task<ServiceOperationResult<AuthResult?>> ReAuthenticateAsync(string refreshToken, string accessToken) {
            var isValidAccessToken = ValidateAccessToken(accessToken, validateLifetime: false);
            if (!isValidAccessToken)
                return ServiceOperationResult<AuthResult?>.Failure(ServiceOperationStatus.Unauthorized, "Invalid access token");

            var jwt = ReadJWT(accessToken);
            if (jwt is null)
                return ServiceOperationResult<AuthResult?>.Failure(ServiceOperationStatus.Unauthorized, "Can't read this token");

            var userId = jwt.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
            if (userId is null)
                return ServiceOperationResult<AuthResult?>.Failure(ServiceOperationStatus.Unauthorized, "User id is null");

            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
                return ServiceOperationResult<AuthResult?>.Failure(ServiceOperationStatus.Unauthorized, "User is not found");

            var currentRefreshToken = await _refreshTokenRepository.GetTableNoTracking()
                                             .FirstOrDefaultAsync(x => x.AccessTokenJTI == jwt.Id &&
                                                                     x.Token == refreshToken &&
                                                                     x.UserId == Guid.Parse(userId));

            if (currentRefreshToken is null || !currentRefreshToken.IsActive)
                return ServiceOperationResult<AuthResult?>.Failure(ServiceOperationStatus.Unauthorized, "Refresh token is not valid");

            //new jwt result
            var jwtResultOperation = await AuthenticateAsync(user, currentRefreshToken.Expires);
            if (jwtResultOperation.Status != ServiceOperationStatus.Succeeded || jwtResultOperation.Data is null)
                return ServiceOperationResult<AuthResult?>.Failure(ServiceOperationStatus.Failed, "Failed to generate new token");

            currentRefreshToken.RevokationDate = DateTime.UtcNow;
            await _refreshTokenRepository.UpdateAsync(currentRefreshToken);
            return jwtResultOperation;
        }

        //public async Task<ServiceOperationResult<JwtResult?>> GoogleAuthenticateAsync(string idToken) {
        //    // Verify Google ID Token
        //    GoogleJsonWebSignature.Payload? payload;
        //    try {
        //        var settings = new GoogleJsonWebSignature.ValidationSettings {
        //            Audience = new[] { _googleAuthSettings.ClientId }
        //        };

        //        payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);
        //    }
        //    catch {
        //        return ServiceOperationResult<JwtResult?>.Failure(ServiceOperationStatus.Unauthorized, "Invalid Google token");
        //    }

        //    if (payload == null || string.IsNullOrEmpty(payload.Email))
        //        return ServiceOperationResult<JwtResult?>.Failure(ServiceOperationStatus.InvalidParameters, "Invalid Google token payload");

        //    // Check if user exists
        //    var user = await _userManager.FindByEmailAsync(payload.Email);

        //    if (user == null) {
        //        // Create new user
        //        user = new ApplicationUser {
        //            Email = payload.Email,
        //            UserName = payload.Email,
        //            FullName = payload.Name ?? payload.Email,
        //            EmailConfirmed = payload.EmailVerified,
        //            IsOnline = false
        //        };

        //        var createResult = await _userManager.CreateAsync(user);
        //        if (!createResult.Succeeded) {
        //            var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
        //            return ServiceOperationResult<JwtResult?>.Failure(ServiceOperationStatus.Failed, $"Failed to create user: {errors}");
        //        }

        //        // Add default role
        //        var roleResult = await _userManager.AddToRoleAsync(user, "User");
        //        if (!roleResult.Succeeded) {
        //            return ServiceOperationResult<JwtResult?>.Failure(ServiceOperationStatus.Failed, "Failed to assign user role");
        //        }
        //    }

        // Generate JWT token
        //    var jwtResult = await AuthenticateAsync(user, null);
        //    if (jwtResult == null)
        //        return ServiceOperationResult<JwtResult?>.Failure(ServiceOperationStatus.Failed, "Failed to generate authentication token");

        //    // Fill user info in result
        //    jwtResult.User = new GetUserByIdResponse {
        //        Id = user.Id,
        //        UserName = user.UserName ?? string.Empty,
        //        Email = user.Email ?? string.Empty,
        //        FullName = user.FullName
        //    };

        //    return ServiceOperationResult<JwtResult?>.Success(jwtResult);
        //}

        public bool ValidateAccessToken(string token, bool validateLifetime = true) {
            var tokenValidationParameters = new TokenValidationParameters {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret)),
                ValidateIssuer = false,
                ValidateAudience = true,
                ValidIssuer = _jwtSettings.Issuer,
                ValidAudience = _jwtSettings.Audience,
                ValidateLifetime = validateLifetime,
                ClockSkew = TimeSpan.FromMinutes(2)  //default = 5 min (security gap)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);

            if (!(securityToken is JwtSecurityToken jwtSecurityToken) ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                return false;

            return principal is null;
        }

        //public async Task<ServiceOperationResult> SendResetPasswordCodeAsync(string email) {
        //    if (email is null)
        //        return ServiceOperationResult.InvalidParameters;

        //    await using var trans = await _context.Database.BeginTransactionAsync();
        //    try {
        //        var user = await _userManager.FindByEmailAsync(email);
        //        if (user is null)
        //            return ServiceOperationResult.Failed;   //return generic error for security

        //        //we should handle old codes before generating a new one
        //        var oldCodes = await _context.ResetPasswordCodes.Where(x => x.UserId == user.Id).ExecuteDeleteAsync();


        //        var code = new Random().Next(100000, 999999).ToString("D6");
        //        var hashedCode = BCrypt.Net.BCrypt.HashPassword(code);
        //        var resetCode = new ResetPasswordCode {
        //            UserId = user.Id,
        //            HashedCode = hashedCode,
        //            ExpiresAt = DateTime.UtcNow.AddMinutes(10),
        //            IsUsed = false
        //        };

        //        await _context.ResetPasswordCodes.AddAsync(resetCode);
        //        await _context.SaveChangesAsync();
        //        await _emailService.SendEmail(email, $"Your Reset Password Code is: {code}", "Reset password");
        //        await trans.CommitAsync();
        //        return ServiceOperationResult.Succeeded;
        //    }
        //    catch {
        //        await trans.RollbackAsync();
        //        return ServiceOperationResult.Failed;
        //    }
        //}

        //public async Task<JwtResult?> VerifyResetPasswordCodeAsync(string email, string code) {
        //    if (email is null || code is null)
        //        return null;

        //    await using var trans = await _context.Database.BeginTransactionAsync();
        //    try {
        //        var user = await _userManager.FindByEmailAsync(email);
        //        if (user is null)
        //            return null;

        //        var resetCode = await _context.ResetPasswordCodes
        //            .FirstOrDefaultAsync(x => x.UserId == user.Id);

        //        if (resetCode == null || !resetCode.IsValid) return null;

        //        bool isCodeValid = BCrypt.Net.BCrypt.Verify(code, resetCode.HashedCode);
        //        if (!isCodeValid) return null;
        //        resetCode.IsUsed = true;
        //        await _context.SaveChangesAsync();

        //        var temporaryToken = await GeneratePasswordResetToken(user);
        //        await trans.CommitAsync();
        //        return temporaryToken;

        //    }
        //    catch {
        //        await trans.RollbackAsync();
        //        return null;
        //    }
        //}


        #region Helper functions
        private async Task<JwtSecurityToken> GenerateAccessToken(ApplicationUser user, List<Claim>? claims = null, DateTime? expDate = null) {
            return new JwtSecurityToken(
                  issuer: _jwtSettings.Issuer,
                  audience: _jwtSettings.Audience,
                  claims: claims ?? await GetUserClaims(user),
                  signingCredentials: GetSigningCredentials(),
                  expires: expDate ?? DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes)
              );
        }
        private async Task<List<Claim>> GetUserClaims(ApplicationUser user) {
            var claims = new List<Claim>()
             {
                 new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
                 new Claim(ClaimTypes.Name,user.UserName),

                 //to make unique token every time
                 new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString())

             };
            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
                claims.Add(new Claim(ClaimTypes.Role, role));

            var customClaims = await _userManager.GetClaimsAsync(user);
            claims.AddRange(customClaims);

            return claims;
        }
        private SigningCredentials GetSigningCredentials() {
            var securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_jwtSettings.Secret));
            return new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        }
        private RefreshTokenDTO GenerateRefreshToken(Guid userId, DateTime? expirationDate = null) {
            var randomBytes = new byte[64];
            RandomNumberGenerator.Fill(randomBytes);
            string refreshTokenValue = Convert.ToBase64String(randomBytes);

            return new RefreshTokenDTO {
                Token = refreshTokenValue,
                ExpirationDate = expirationDate ?? DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays),
                CreatedAt = DateTime.UtcNow,
                UserId = userId
            };


        }
        private async Task AddRefreshTokenToDatabase(RefreshTokenDTO refreshTokenDTO, string accessTokenJti) {
            var refreshToken = new RefreshToken {
                Created = DateTime.UtcNow,
                Expires = refreshTokenDTO.ExpirationDate,
                AccessTokenJTI = accessTokenJti,
                Token = refreshTokenDTO.Token,
                UserId = refreshTokenDTO.UserId
            };
            await _refreshTokenRepository.AddAsync(refreshToken);
        }
        private JwtSecurityToken ReadJWT(string accessToken) {
            if (string.IsNullOrEmpty(accessToken)) {
                throw new ArgumentNullException(nameof(accessToken));
            }
            var handler = new JwtSecurityTokenHandler();
            var response = handler.ReadJwtToken(accessToken);
            return response;
        }

        private async Task<AuthResult?> GeneratePasswordResetToken(ApplicationUser user) {
            if (user is null)
                return null;

            var userClaims = await GetUserClaims(user);
            userClaims.Add(new Claim("purpose", "reset-password"));
            var jwtSecurityToken = await GenerateAccessToken(user, userClaims, DateTime.UtcNow.AddMinutes(5));

            AuthResult jwtResult = new AuthResult {
                AccessToken = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
            };
            return jwtResult;
        }

        public Task<ServiceOperationResult<AuthResult?>> GoogleAuthenticateAsync(string idToken) {
            throw new NotImplementedException();
        }


        #endregion

    }
}
