using YallaKhadra.Core.Enums;

namespace YallaKhadra.Core.Features.Orders.Queries.Responses {
    public class GetMyOrdersResponse {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public int TotalPoints { get; set; }
        public OrderStatus Status { get; set; }
        public string StatusName { get; set; } = string.Empty;
        public int ItemsCount { get; set; }
    }
}
