using YallaKhadra.Core.Entities.IdentityEntities;
using YallaKhadra.Core.Enums;

namespace YallaKhadra.Core.Abstracts.ApiAbstracts;

public interface ICurrentUserService {
    Guid? UserId { get; }
    string? UserName { get; }
    bool IsAuthenticated { get; }
    Task<ApplicationUser?> GetCurrentUserAsync();
    IList<UserRole> GetRoles();
    bool IsInRole(UserRole roleName);
}