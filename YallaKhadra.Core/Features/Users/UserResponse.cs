using System.Text.Json.Serialization;

namespace YallaKhadra.Core.Features.Users {
    public class UserResponse {

        [JsonPropertyOrder(-1)]
        public int Id { get; set; }
        public string Email { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int PointsBalance { get; set; }



    }
}
