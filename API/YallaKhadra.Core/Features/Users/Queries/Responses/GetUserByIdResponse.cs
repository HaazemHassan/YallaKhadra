namespace YallaKhadra.Core.Features.Users.Queries.Responses {
    public class GetUserByIdResponse {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Country { get; set; }
        public string PhoneNumber { get; set; }

    }
}
