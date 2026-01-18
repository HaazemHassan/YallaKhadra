using Microsoft.AspNetCore.Identity;

namespace YallaKhadra.Core.Entities.IdentityEntities {
    public class ApplicationUser : IdentityUser<int> {
        public ApplicationUser() {
            RefreshTokens = new HashSet<RefreshToken>();
        }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? Address { get; set; }
        public int PointsBalance { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public virtual ICollection<RefreshToken> RefreshTokens { get; set; }

    }
}
