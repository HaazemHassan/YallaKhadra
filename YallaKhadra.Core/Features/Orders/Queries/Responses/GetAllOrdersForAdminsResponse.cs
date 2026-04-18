using YallaKhadra.Core.Enums;

namespace YallaKhadra.Core.Features.Orders.Queries.Responses {
    public class GetAllOrdersForAdminsResponse {
        public int Id { get; set; }
        public string UserFullName { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public int ItemsCount { get; set; }
        public int TotalPoints { get; set; }
        public OrderStatus Status { get; set; }
    }
}
