using YallaKhadra.Core.Entities.IdentityEntities;
using YallaKhadra.Core.Enums;

namespace YallaKhadra.Core.Entities
{
    public class WasteReport : BaseEntity<int>
    {

        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public string? Address { get; set; }
        public ReportStatus Status { get; set; } = ReportStatus.Pending;
        public WasteType WasteType { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int UserId { get; set; }

        public virtual ApplicationUser User { get; set; } = null!;

        public virtual ICollection<ReportImage> Images { get; set; } = new HashSet<ReportImage>();
    }
}
