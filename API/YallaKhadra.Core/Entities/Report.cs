using YallaKhadra.Core.Entities.IdentityEntities;
using YallaKhadra.Core.Enums;

namespace YallaKhadra.Core.Entities
{
    public class Report : BaseEntity<int>
    {
        //User Information
        public Guid UserId { get; set; }
        public ApplicationUser User { get; set; }

        //Report Details
        public string Description { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Address { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public ReportStatus Status { get; set; } = ReportStatus.Pending;
        public int PointsAwarded { get; set; } = 0;

        //Admin Review
        public DateTime? ReviewedAt { get; set; }
        public string? Notes { get; set; }
        public Guid? ReviewedById { get; set; }
        public ApplicationUser? ReviewedBy { get; set; }

        //photos
        public ICollection<Photo>? Photos { get; set; } = new List<Photo>();
    }
}
