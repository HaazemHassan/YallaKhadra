using YallaKhadra.Core.Entities.BaseEntities;

namespace YallaKhadra.Core.Entities.GreenEntities
{
    public class ReportImage : BaseImage
    {
        public int ReportId { get; set; }

        public WasteReport Report { get; set; } = null!;
    }
}
