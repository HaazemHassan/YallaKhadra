using YallaKhadra.Core.Entities.IdentityEntities;

namespace YallaKhadra.Core.Abstracts.ServicesContracts;

public interface ICurrentUserService {
    Guid? UserId { get; }
    string? UserName { get; }
    bool IsAuthenticated { get; }
    Task<ApplicationUser?> GetCurrentUserAsync();
}