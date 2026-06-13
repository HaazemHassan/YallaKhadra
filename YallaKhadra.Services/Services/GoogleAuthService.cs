using Google.Apis.Auth;
using Microsoft.Extensions.Options;
using YallaKhadra.Core.Abstracts.ServicesContracts;
using YallaKhadra.Core.Bases;
using YallaKhadra.Core.Bases.Options;
using YallaKhadra.Core.Enums;
using YallaKhadra.Core.Features.Authentication.Common;

namespace YallaKhadra.Services.Services {
    public class GoogleAuthService : IGoogleAuthService {

        private readonly GoogleAuthOptions _googleOptions;

        public GoogleAuthService(IOptions<GoogleAuthOptions> googleOptions) {
            _googleOptions = googleOptions.Value;
        }

        public async Task<ServiceOperationResult<GoogleUserInfo?>> ValidateIdTokenAsync(string idToken, CancellationToken ct = default) {
            try {
                var settings = new GoogleJsonWebSignature.ValidationSettings {
                    Audience = [_googleOptions.ClientId],
                    ExpirationTimeClockTolerance = TimeSpan.FromMinutes(1),
                    IssuedAtClockTolerance = TimeSpan.FromMinutes(1)
                };

                var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);

                if (!payload.EmailVerified) {
                    return ServiceOperationResult<GoogleUserInfo?>.Failure(
                        ServiceOperationStatus.InvalidParameters,
                        "Google account email is not verified.");
                }

                var firstName =
                    payload.GivenName ??
                    payload.Name?.Split(' ').FirstOrDefault() ??
                    "User";

                var lastName =
                    payload.FamilyName ??
                    payload.Name?.Split(' ').LastOrDefault() ??
                    string.Empty;

                return ServiceOperationResult<GoogleUserInfo?>.Success(new GoogleUserInfo(
                    payload.Subject,
                    payload.Email,
                    firstName,
                    lastName,
                    payload.EmailVerified
                ));
            }
            catch (InvalidJwtException) {
                return ServiceOperationResult<GoogleUserInfo?>.Failure(
                    ServiceOperationStatus.Unauthorized,
                    "Invalid ID token.");
            }
        }
    }
}
