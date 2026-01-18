using System.Linq.Expressions;
using YallaKhadra.Core.Bases;
using YallaKhadra.Core.Entities.IdentityEntities;

namespace YallaKhadra.Core.Abstracts.ServicesContracts {
    public interface IApplicationUserService {
        public Task<bool> IsUserExist(Expression<Func<ApplicationUser, bool>> predicate);
        public Task<ServiceOperationResult<ApplicationUser?>> AddUser(ApplicationUser user, string password);
        //public Task<bool> SendConfirmationEmailAsync(ApplicationUser user);
        //public Task<ServiceOperationResult<string?>> ConfirmEmailAsync(int userId, string code);
        //public Task<ServiceOperationResult<string?>> ResetPasswordAsync(ApplicationUser user, string newPassword);
        public Task<string?> GetFullName(int userId);


    }
}
