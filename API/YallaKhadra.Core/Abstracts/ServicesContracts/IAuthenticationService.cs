﻿using YallaKhadra.Core.Bases;
using YallaKhadra.Core.Bases.Authentication;
using YallaKhadra.Core.Entities.IdentityEntities;

namespace YallaKhadra.Core.Abstracts.ServicesContracts {
    public interface IAuthenticationService {
        public Task<ServiceOperationResult<AuthResult?>> AuthenticateAsync(ApplicationUser user, DateTime? refreshTokenExpDate = null);
        public bool ValidateAccessToken(string token, bool validateLifetime = true);
        public Task<ServiceOperationResult<AuthResult?>> ReAuthenticateAsync(string refreshToken, string accessToken);
        public Task<ServiceOperationResult<AuthResult?>> GoogleAuthenticateAsync(string idToken);
        //public Task<ServiceOperationResult> SendResetPasswordCodeAsync(string email);
        //public Task<ServiceOperationResult<JwtResult?>> VerifyResetPasswordCodeAsync(string email, string code);
    }
}
