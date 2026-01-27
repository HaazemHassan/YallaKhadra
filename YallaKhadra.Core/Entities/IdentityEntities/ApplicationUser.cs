using Microsoft.AspNetCore.Identity;
using YallaKhadra.Core.Entities.PointsEntities;

namespace YallaKhadra.Core.Entities.IdentityEntities {
    public class ApplicationUser : IdentityUser<int> {
        public ApplicationUser() {
            RefreshTokens = new HashSet<RefreshToken>();
            PointsTransactions = new HashSet<PointsTransaction>();
        }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? Address { get; set; }
        public int PointsBalance { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public virtual ICollection<RefreshToken> RefreshTokens { get; set; }
        public virtual ICollection<PointsTransaction> PointsTransactions { get; set; }

    }
}
