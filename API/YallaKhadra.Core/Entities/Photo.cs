using YallaKhadra.Core.Entities.IdentityEntities;
using YallaKhadra.Core.Enums;

namespace YallaKhadra.Core.Entities
{
    public class Photo : BaseEntity<int>
    {
        public string Url { get; set; }
        public string PublicId { get; set; }
        public PhotoType Type { get; set; }


        public int? ReportId { get; set; }
        public Report? Report { get; set; }

        public Guid? UserId { get; set; }
        public ApplicationUser? User { get; set; }
    }
}
