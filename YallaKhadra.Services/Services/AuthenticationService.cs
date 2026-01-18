using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using YallaKhadra.Core.Abstracts.ApiAbstracts;
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
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IApplicationUserService _applicationUserService;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;
        //private readonly IEmailService _emailService;



        public AuthenticationService(JwtSettings jwtSettings, UserManager<ApplicationUser> userManager, IRefreshTokenRepository refreshTokenRepository, IApplicationUserService applicationUserService, IMapper mapper /*IEmailService emailService*/, RoleManager<ApplicationRole> roleManager, ICurrentUserService currentUserService) {
            _jwtSettings = jwtSettings;
            _userManager = userManager;
            _refreshTokenRepository = refreshTokenRepository;
            _applicationUserService = applicationUserService;
            _mapper = mapper;
            _roleManager = roleManager;
            _currentUserService = currentUserService;
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

            var currentRefreshToken = await _refreshTokenRepository.GetTableAsTracking()
                                             .FirstOrDefaultAsync(x => x.AccessTokenJTI == jwt.Id &&
                                                                     x.Token == refreshToken &&
                                                                     x.UserId == int.Parse(userId));

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

            return principal is not null;
        }



        public async Task<ServiceOperationResult<bool>> LogoutAsync(string refreshToken) {
            if (!_currentUserService.IsAuthenticated)
                return (ServiceOperationResult<bool>.Failure(ServiceOperationStatus.Unauthorized, "You're already signed out!"));

            int userId = _currentUserService.UserId!.Value;

            var refreshTokenFromDb = await _refreshTokenRepository.GetTableAsTracking()
                                             .FirstOrDefaultAsync(r => r.Token == refreshToken && r.UserId == userId);

            if (refreshTokenFromDb == null || !refreshTokenFromDb.IsActive)
                return ServiceOperationResult<bool>.Failure(ServiceOperationStatus.NotFound, "You maybe signed out!");

            refreshTokenFromDb.RevokationDate = DateTime.UtcNow;
            await _refreshTokenRepository.UpdateAsync(refreshTokenFromDb);
            return ServiceOperationResult<bool>.Success(true);

        }

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
                 new Claim(ClaimTypes.Email,user.Email!),

                 //to make unique token every time
                 new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString())

             };

            var customClaims = await _userManager.GetClaimsAsync(user);
            claims.AddRange(customClaims);

            var roles = await _userManager.GetRolesAsync(user);
            foreach (var roleName in roles) {
                claims.Add(new Claim(ClaimTypes.Role, roleName));

                var role = await _roleManager.FindByNameAsync(roleName);
                if (role == null) continue;

                var roleClaims = await _roleManager.GetClaimsAsync(role);
                claims.AddRange(roleClaims);
            }

            return claims;
        }
        private SigningCredentials GetSigningCredentials() {
            var securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_jwtSettings.Secret));
            return new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        }
        private RefreshTokenDTO GenerateRefreshToken(int userId, DateTime? expirationDate = null) {
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

        #endregion

    }
}
