using YallaKhadra.Core.Enums;

namespace YallaKhadra.Core.Features.Reports.Queries.Responses
{
    public class GetMyReportsPaginatedResponse
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Address { get; set; }
        public DateTime CreatedAt { get; set; }
        public ReportStatus Status { get; set; }
        public int PointsAwarded { get; set; }
        public DateTime? ReviewedAt { get; set; }
        public string? Notes { get; set; }
        public string? ReviewedByName { get; set; }
        public List<ReportPhotoDto> Photos { get; set; } = new List<ReportPhotoDto>();
    }
}
