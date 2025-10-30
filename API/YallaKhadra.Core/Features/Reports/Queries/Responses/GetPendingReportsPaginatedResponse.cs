using YallaKhadra.Core.Enums;

namespace YallaKhadra.Core.Features.Reports.Queries.Responses
{
    public class GetPendingReportsPaginatedResponse
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string UserFullName { get; set; }
        public string Description { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Address { get; set; }
        public DateTime CreatedAt { get; set; }
        public ReportStatus Status { get; set; }
        public int PointsAwarded { get; set; }
        public List<ReportPhotoDto> Photos { get; set; } = new List<ReportPhotoDto>();
    }

    public class ReportPhotoDto
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public string PublicId { get; set; }
        public PhotoType Type { get; set; }
    }
}
