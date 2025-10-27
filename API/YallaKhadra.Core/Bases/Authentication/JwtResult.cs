using YallaKhadra.Core.Features.Users.Queries.Responses;

namespace YallaKhadra.Core.Bases.Authentication {
    public class JwtResult {
        public string AccessToken { get; set; }
        public RefreshTokenDTO? RefreshToken { get; set; }

        public GetUserByIdResponse User { get; set; }
    }

    public class RefreshTokenDTO {
        public string Token { get; set; }
        public Guid UserId { get; set; }
        public DateTime ExpirationDate { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
