namespace YallaKhadra.Core.Features.WasteReports.Queries.Responses
{
    public class MyWorkResponse
    {
        public int ReportId { get; set; }
        public string? FirstImageUrl { get; set; }
        public string? Address { get; set; }
        public DateTime CompletedAt { get; set; }
        public TimeSpan Duration { get; set; }
    }
}
