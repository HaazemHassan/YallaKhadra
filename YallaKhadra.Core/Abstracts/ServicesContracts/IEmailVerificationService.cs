using YallaKhadra.Core.Bases;
using YallaKhadra.Core.Entities.IdentityEntities;

namespace YallaKhadra.Core.Abstracts.ServicesContracts {
    public interface IEmailVerificationService {
        Task<ServiceOperationResult<string?>> CreateEmailConfirmationCodeAsync(int applicationUserId);
        Task SendConfirmationEmailAsync(ApplicationUser user, string code);
        Task<ServiceOperationResult> ConfirmEmailAsync(int applicationUserId, string code);
    }
}
