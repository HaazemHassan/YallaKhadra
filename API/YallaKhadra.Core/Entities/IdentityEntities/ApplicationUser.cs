using Microsoft.AspNetCore.Identity;

namespace YallaKhadra.Core.Entities.IdentityEntities {
    public class ApplicationUser : IdentityUser<Guid> {
        public ApplicationUser() {
            RefreshTokens = new HashSet<RefreshToken>();
        }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? Address { get; set; }
        public string? Country { get; set; }

        public virtual ICollection<RefreshToken> RefreshTokens { get; set; }

    }
}
