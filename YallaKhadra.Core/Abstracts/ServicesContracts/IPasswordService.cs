using YallaKhadra.Core.Bases;
using YallaKhadra.Core.Entities.IdentityEntities;

namespace YallaKhadra.Core.Abstracts.ServicesContracts
{
    public interface IPasswordService
    {
        Task<ServiceOperationResult<string?>> CreatePasswordResetCodeAsync(int applicationUserId);
        Task SendPasswordResetEmailAsync(ApplicationUser user, string code);
        Task<ServiceOperationResult<bool>> IsPasswordResetCodeValidAsync(string email, string code);
        Task<ServiceOperationResult> ResetPasswordAsync(string email, string code, string newPassword);
    }
}
