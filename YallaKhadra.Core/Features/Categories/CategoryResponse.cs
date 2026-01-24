using System.Text.Json.Serialization;

namespace YallaKhadra.Core.Features.Categories {
    public class CategoryResponse {
        [JsonPropertyOrder(-1)]
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
