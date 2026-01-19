using YallaKhadra.Core.Enums;

namespace YallaKhadra.Core.Features.WasteReports.Queries.Responses {
    public class WasteReportResponse {
        public int Id { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public string? Address { get; set; }
        public ReportStatus Status { get; set; }
        public WasteType WasteType { get; set; }
        public DateTime CreatedAt { get; set; }
        public int UserId { get; set; }
        public string? UserName { get; set; }
        public List<ReportImageDto> Images { get; set; } = new List<ReportImageDto>();
    }

    public class ReportImageDto {
        public int Id { get; set; }
        public string Url { get; set; } = string.Empty;
        public DateTime UploadedAt { get; set; }
    }
}
