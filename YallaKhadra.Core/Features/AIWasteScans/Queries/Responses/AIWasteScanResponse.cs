namespace YallaKhadra.Core.Features.AIWasteScans.Queries.Responses {
    public class AIWasteScanResponse {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string? UserName { get; set; }
        public string? AIPredictedType { get; set; }
        public bool AIIsRecyclable { get; set; }
        public string? AIExplanation { get; set; }
        public DateTime CreatedAt { get; set; }
        public WasteScanImageDto? WasteScanImage { get; set; }
    }

    public class WasteScanImageDto {
        public int Id { get; set; }
        public string Url { get; set; } = string.Empty;
        public DateTime UploadedAt { get; set; }
    }
}
