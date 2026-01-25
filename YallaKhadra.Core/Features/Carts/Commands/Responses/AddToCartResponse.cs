namespace YallaKhadra.Core.Features.Carts.Commands.Responses {
    public class AddToCartResponse {
        public int CartItemId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public int PointsCost { get; set; }
        public int TotalPoints { get; set; }
    }
}
