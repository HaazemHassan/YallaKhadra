namespace YallaKhadra.Core.Features.CleanupTasks.Queries.Responses {
    public class CleanupWorkOverviewResponse {
        public int CompletedCleanupsCount { get; set; }
        public double TotalHours { get; set; }
        public decimal TotalWeightInKg { get; set; }
    }
}
