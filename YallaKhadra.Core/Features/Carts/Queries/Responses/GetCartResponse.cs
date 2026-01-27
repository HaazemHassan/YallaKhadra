namespace YallaKhadra.Core.Features.Carts.Queries.Responses {
    public class GetCartResponse {
        public int CartId { get; set; }
        public int UserId { get; set; }
        public List<CartItemDto> Items { get; set; } = new();
        public int TotalPoints { get; set; }
        public int TotalPointsWithCurrentPrices { get; set; }
        public bool HasPriceChanges { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
