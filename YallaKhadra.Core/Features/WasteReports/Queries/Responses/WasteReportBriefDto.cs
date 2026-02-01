using YallaKhadra.Core.Enums;

namespace YallaKhadra.Core.Features.WasteReports.Queries.Responses {
    public class WasteReportBriefDto {
        public int Id { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public string? Address { get; set; }
        public ReportStatus Status { get; set; }
        public string StatusName { get; set; } = string.Empty;
        public WasteType WasteType { get; set; }
        public string WasteTypeName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public int UserId { get; set; }
        public string? UserName { get; set; }
        
        public ReportImageDto? FirstImage { get; set; }
    }
}
