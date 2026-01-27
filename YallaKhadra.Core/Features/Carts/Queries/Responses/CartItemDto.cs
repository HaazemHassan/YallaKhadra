namespace YallaKhadra.Core.Features.Carts.Queries.Responses {
    public class CartItemDto {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ProductDescription { get; set; } = string.Empty;
        public string? ProductMainImageUrl { get; set; }
        public int Quantity { get; set; }
        public int PointsCost { get; set; }
        public int CurrentProductPointsCost { get; set; }
        public bool PriceChanged { get; set; }
        public int TotalPoints { get; set; }
        public int TotalPointsWithCurrentPrice { get; set; }
        public bool IsSelected { get; set; }
        public DateTime AddedAt { get; set; }
    }
}
