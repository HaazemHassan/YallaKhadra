namespace YallaKhadra.Core.Features.Leaderboard.Queries.Responses
{
    public class LeaderboardEntryResponse
    {
        public int UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string? ProfileImageUrl { get; set; }
        public int TotalReportsCount { get; set; }
        public int Rank { get; set; }
    }
}
