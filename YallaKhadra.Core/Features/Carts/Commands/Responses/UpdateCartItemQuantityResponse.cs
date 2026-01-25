namespace YallaKhadra.Core.Features.Carts.Commands.Responses {
    public class UpdateCartItemQuantityResponse {
        public int CartItemId { get; set; }
        public int Quantity { get; set; }
        public int PointsCost { get; set; }
        public int TotalPoints { get; set; }
    }
}
