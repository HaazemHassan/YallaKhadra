using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using YallaKhadra.Core.Abstracts.ServicesContracts;
using YallaKhadra.Core.Entities.IdentityEntities;

namespace YallaKhadra.Services.Services {
    public class CurrentUserService : ICurrentUserService {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<ApplicationUser> _userManager;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor, UserManager<ApplicationUser> userManager) {
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }

        public Guid? UserId {
            get {
                var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                return Guid.TryParse(userIdClaim, out var userId) ? userId : null;
            }
        }

        public string? UserName =>
            _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;

        public bool IsAuthenticated =>
            _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;

        public async Task<ApplicationUser?> GetCurrentUserAsync() {
            var principal = _httpContextAccessor.HttpContext?.User;
            if (principal == null) return null;

            return await _userManager.GetUserAsync(principal);
        }


    }
}