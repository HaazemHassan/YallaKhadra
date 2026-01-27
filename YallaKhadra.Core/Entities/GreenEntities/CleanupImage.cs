using YallaKhadra.Core.Entities.BaseEntities;

namespace YallaKhadra.Core.Entities.GreenEntities
{
    public class CleanupImage : BaseImage
    {
        public int CleanupTaskId { get; set; }
        public CleanupTask CleanupTask { get; set; } = null!;
    }
}
