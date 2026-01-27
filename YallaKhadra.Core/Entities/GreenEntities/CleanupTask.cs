using YallaKhadra.Core.Entities.BaseEntities;
using YallaKhadra.Core.Entities.IdentityEntities;
using YallaKhadra.Core.Enums;

namespace YallaKhadra.Core.Entities.GreenEntities
{
    public class CleanupTask : BaseEntity<int>
    {
        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
        public DateTime? CompletedAt { get; set; }

        public WasteType? FinalWasteType { get; set; }
        public decimal? FinalWeightInKg { get; set; }

        public int WorkerId { get; set; }
        public int ReportId { get; set; }

        public ApplicationUser Worker { get; set; } = null!;
        public WasteReport Report { get; set; } = null!;
        public virtual ICollection<CleanupImage> Images { get; set; } = new HashSet<CleanupImage>();
    }
}
