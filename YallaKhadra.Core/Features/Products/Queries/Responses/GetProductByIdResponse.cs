using YallaKhadra.Core.Features.Products.Commands.Responses;

namespace YallaKhadra.Core.Features.Products.Queries.Responses {
    public class GetProductByIdResponse {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int PointsCost { get; set; }
        public int Stock { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<ProductImageDto> Images { get; set; } = new();
    }
}
