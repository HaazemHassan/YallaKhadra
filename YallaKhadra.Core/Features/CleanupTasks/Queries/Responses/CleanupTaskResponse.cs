using YallaKhadra.Core.Enums;
using YallaKhadra.Core.Features.WasteReports.Queries.Responses;

namespace YallaKhadra.Core.Features.CleanupTasks.Queries.Responses {
    public class CleanupTaskResponse {
        public int Id { get; set; }
        public DateTime AssignedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public WasteType FinalWasteType { get; set; }
        public string FinalWasteTypeName { get; set; } = string.Empty;
        public decimal FinalWeightInKg { get; set; }
        public int WorkerId { get; set; }
        public string? WorkerName { get; set; }
        public int ReportId { get; set; }
        public WasteReportResponse? Report { get; set; }
        public List<CleanupImageDto> Images { get; set; } = new List<CleanupImageDto>();
    }

    public class CleanupImageDto {
        public int Id { get; set; }
        public string Url { get; set; } = string.Empty;
        public DateTime UploadedAt { get; set; }
    }
}
