using YallaKhadra.Core.Enums;

namespace YallaKhadra.Core.Features.Orders.Queries.Responses {
    public class GetOrderDetailsResponse {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public int TotalPoints { get; set; }
        public OrderStatus Status { get; set; }
        public string StatusName { get; set; } = string.Empty;
        public ShippingDetailsDto ShippingDetails { get; set; } = null!;
        public List<OrderItemDto> OrderItems { get; set; } = new();
    }
}
