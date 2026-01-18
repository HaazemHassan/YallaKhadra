using System.Text.Json.Serialization;
using YallaKhadra.Core.Features.Users.Queries.Responses;

namespace YallaKhadra.Core.Bases.Authentication {
    public class AuthResult {
        public string AccessToken { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public RefreshTokenDTO? RefreshToken { get; set; }

        public GetUserByIdResponse User { get; set; }
    }


    public class RefreshTokenDTO {
        public string Token { get; set; }
        public int UserId { get; set; }
        public DateTime ExpirationDate { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
