using YallaKhadra.Core.Bases;
using YallaKhadra.Core.Features.Authentication.Common;

namespace YallaKhadra.Core.Abstracts.ServicesContracts {
    public interface IGoogleAuthService {
        Task<ServiceOperationResult<GoogleUserInfo?>> ValidateIdTokenAsync(string idToken, CancellationToken ct = default);
    }
}
