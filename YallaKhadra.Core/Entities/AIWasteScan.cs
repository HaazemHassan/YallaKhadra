using YallaKhadra.Core.Entities.IdentityEntities;

namespace YallaKhadra.Core.Entities
{
    public class AIWasteScan:BaseEntity<int>
    {
        public int UserId { get; set; }
        public string? AIPredictedType { get; set; }
        public bool AIIsRecyclable { get; set; }
        public string? AIExplanation { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public virtual WasteScanImage WasteScanImage { get; set; } = null!;
        public virtual ApplicationUser User { get; set; } = null!;
    }
}
