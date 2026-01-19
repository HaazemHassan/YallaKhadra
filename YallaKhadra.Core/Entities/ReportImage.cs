namespace YallaKhadra.Core.Entities
{
    public class ReportImage : BaseImage
    {
        public int ReportId { get; set; }

        public WasteReport Report { get; set; } = null!;
    }
}
