using YallaKhadra.Core.Entities.BaseEntities;
using YallaKhadra.Core.Entities.IdentityEntities;
using YallaKhadra.Core.Enums;

namespace YallaKhadra.Core.Entities.PointsEntities
{
    public class PointsTransaction : BaseEntity<int>
    {
        public int Points { get; set; }
        public PointsTransactionType TransactionType { get; set; }
        public PointsSourceType pointsSource { get; set; }
        public int SourceId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int UserId { get; set; }
        public virtual ApplicationUser User { get; set; } = null!;
    }
}
