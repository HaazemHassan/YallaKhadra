using System.ComponentModel.DataAnnotations.Schema;

namespace YallaKhadra.Core.Entities.IdentityEntities {
    public class RefreshToken {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string? AccessTokenJTI { get; set; }
        public string Token { get; set; }
        public DateTime Expires { get; set; }
        public bool IsExpired => DateTime.UtcNow >= Expires;
        public DateTime Created { get; set; } = DateTime.UtcNow;
        public DateTime? RevokationDate { get; set; }
        public bool IsActive => RevokationDate is null && !IsExpired;

        [ForeignKey(nameof(UserId))]
        public ApplicationUser? User { get; set; }
    }
}
