using YallaKhadra.Core.Entities.IdentityEntities;

namespace YallaKhadra.Core.Abstracts.ApiAbstracts;

public interface ICurrentUserService {
    Guid? UserId { get; }
    string? UserName { get; }
    bool IsAuthenticated { get; }
    Task<ApplicationUser?> GetCurrentUserAsync();
}