using YallaKhadra.Core.Enums;

namespace YallaKhadra.Core.Features.Orders.Commands.Responses {
    public class PlaceOrderResponse {
        public int OrderId { get; set; }
        public OrderStatus Status { get; set; }
        public string StatusName { get; set; } = string.Empty;
        public int TotalPoints { get; set; }
        public DateTime OrderDate { get; set; }
        public int ItemsCount { get; set; }
    }
}
