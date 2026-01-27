using YallaKhadra.Core.Entities.BaseEntities;

namespace YallaKhadra.Core.Entities.GreenEntities
{
    public class WasteScanImage : BaseImage
    {
        public int AIWasteScanId { get; set; }
        public virtual AIWasteScan AIWasteScan { get; set; } = null!;
    }
}
