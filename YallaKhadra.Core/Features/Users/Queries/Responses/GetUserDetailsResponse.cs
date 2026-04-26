namespace YallaKhadra.Core.Features.Users.Queries.Responses {
    public class GetUserDetailsResponse {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public UserProfileImageDto? ProfileImage { get; set; }
        public string PhoneNumber { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public int Points { get; set; }
        public int PendingReportsCount { get; set; }
        public int InProgressReportsCount { get; set; }
        public int DoneReportsCount { get; set; }
    }
}
