namespace YallaKhadra.Core.Features.Dashboard.Queries.Responses
{
    public class DashboardTrendsResponse
    {
        public List<TrendDataPoint> ReportsTrend { get; set; } = new();
        public List<TrendDataPoint> OrdersTrend { get; set; } = new();
    }

    public class TrendDataPoint
    {
        public string DateLabel { get; set; } = string.Empty;
        public decimal Value { get; set; }
    }
}
