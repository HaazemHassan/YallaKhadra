using YallaKhadra.Core.Entities.BaseEntities;

namespace YallaKhadra.Core.Entities.IdentityEntities {
    public class UserProfileImage : BaseImage {
        public int UserId { get; set; }
        public ApplicationUser User { get; set; } = null!;
    }
}
