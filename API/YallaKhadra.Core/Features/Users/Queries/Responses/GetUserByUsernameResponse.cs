namespace YallaKhadra.Core.Features.Users.Queries.Responses {
    public class GetUserByUsernameResponse {
        public int Id { get; set; }
        public string Username { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
    }
}