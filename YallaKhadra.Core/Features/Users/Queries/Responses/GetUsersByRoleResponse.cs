namespace YallaKhadra.Core.Features.Users.Queries.Responses
{
    public class GetUsersByRoleResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public bool IsLocked { get; set; }
        public int? Points { get; set; }
    }
}
