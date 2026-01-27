using YallaKhadra.Core.Enums;

namespace YallaKhadra.Core.Entities.E_CommerceEntities {
    public class Order : BaseEntity<int> {
        public int UserId { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public int TotalPoints { get; set; }
        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        public int ShippingDetailsId { get; set; }
        public virtual OrderShippingDetails ShippingDetails { get; set; } = null!;

        public virtual ICollection<OrderItem> OrderItems { get; set; } = new HashSet<OrderItem>();

        public static Order CreateOrder(
            int userId,
            List<OrderItem> orderItems,
            OrderShippingDetails shippingDetails) {

            var totalPoints = orderItems.Sum(item => item.Quantity * item.UnitPointsAtPurchase);

            var order = new Order {
                UserId = userId,
                OrderDate = DateTime.UtcNow,
                TotalPoints = totalPoints,
                Status = OrderStatus.Pending,
                OrderItems = orderItems,
                ShippingDetails = shippingDetails
            };

            return order;
        }

        public void UpdateStatus(OrderStatus newStatus) {
            Status = newStatus;
        }

        public void CancelOrder() {
            if (!CanBeCanceled())
                throw new InvalidOperationException("Cannot cancel order in current status.");

            Status = OrderStatus.Canceled;
        }

        public bool CanBeCanceled() {
            return Status == OrderStatus.Processing || Status == OrderStatus.Pending;
        }

        public int GetItemsCount() {
            return OrderItems.Count;
        }
    }
}