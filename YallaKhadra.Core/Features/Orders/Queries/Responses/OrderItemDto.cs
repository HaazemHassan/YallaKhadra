using YallaKhadra.Core.Enums;

namespace YallaKhadra.Core.Features.Orders.Queries.Responses {
    public class OrderItemDto {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ProductDescription { get; set; } = string.Empty;
        public string? ProductMainImageUrl { get; set; }
        public int Quantity { get; set; }
        public int UnitPointsAtPurchase { get; set; }
        public int TotalPoints { get; set; }
    }
}
