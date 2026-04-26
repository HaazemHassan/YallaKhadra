namespace YallaKhadra.Core.Features.Users.Queries.Responses {
    public class GetWorkerDetailsResponse {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public UserProfileImageDto? ProfileImage { get; set; }
        public string PhoneNumber { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public int TotalCleanups { get; set; }
        public double AvgResponseTime { get; set; }
        public double TotalHours { get; set; }
    }
}
