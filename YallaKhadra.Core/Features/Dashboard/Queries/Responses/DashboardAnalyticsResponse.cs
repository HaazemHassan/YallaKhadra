namespace YallaKhadra.Core.Features.Dashboard.Queries.Responses
{
    public class DashboardAnalyticsResponse
    {
        public ReportsAnalyticsResponse ReportsAnalytics { get; set; } = new();
        public UsersOverviewResponse UsersOverview { get; set; } = new();
        public ECommerceAnalyticsResponse ECommerceAnalytics { get; set; } = new();
    }

    public class ReportsAnalyticsResponse
    {
        public int TotalReports { get; set; }
        public int PendingReports { get; set; }
        public int InProgressReports { get; set; }
        public int CompletedReports { get; set; }
        public decimal WasteCollectedInKg { get; set; }
        public int AiScans { get; set; }
    }

    public class UsersOverviewResponse
    {
        public int TotalUsers { get; set; }
        public int Workers { get; set; }
        public int Admins { get; set; }
    }

    public class ECommerceAnalyticsResponse
    {
        public int Categories { get; set; }
        public int Products { get; set; }
        public int Orders { get; set; }
        public int ItemsSold { get; set; }
    }
}
