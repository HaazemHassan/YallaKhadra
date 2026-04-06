namespace YallaKhadra.Infrastructure.BackgroundJobs {
    public class HangfireOptions {
        public const string SectionName = "HangfireSettings";

        public string DashboardPath { get; set; } = "/jobs";
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
