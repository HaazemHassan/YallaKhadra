namespace YallaKhadra.Core.Features.Users.Queries.Responses {
    public class SearchUsersResponse {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
    }
}