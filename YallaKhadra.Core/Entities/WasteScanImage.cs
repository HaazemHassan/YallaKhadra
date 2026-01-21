namespace YallaKhadra.Core.Entities
{
    public class WasteScanImage : BaseImage
    {
        public int AIWasteScanId { get; set; }
        public virtual AIWasteScan AIWasteScan { get; set; } = null!;
    }
}
