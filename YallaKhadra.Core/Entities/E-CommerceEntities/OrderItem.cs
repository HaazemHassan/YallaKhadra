using YallaKhadra.Core.Entities.BaseEntities;

namespace YallaKhadra.Core.Entities.E_CommerceEntities {
    public class OrderItem : BaseEntity<int> {
        public int OrderId { get; set; }
        public virtual Order Order { get; set; } = null!;

        public int ProductId { get; set; }
        public virtual Product Product { get; set; } = null!;

        public int Quantity { get; set; }
        public int UnitPointsAtPurchase { get; set; }

        public static OrderItem CreateFromCartItem(CartItem cartItem) {
            if (cartItem is null)
                throw new ArgumentNullException(nameof(cartItem));

            if (cartItem.Product is null)
                throw new ArgumentNullException(nameof(cartItem.Product));

            return new OrderItem {
                ProductId = cartItem.ProductId,
                Quantity = cartItem.Quantity,
                UnitPointsAtPurchase = cartItem.Product.PointsCost,
                Product = cartItem.Product
            };
        }

        public int CalculateTotalPoints() {
            return Quantity * UnitPointsAtPurchase;
        }
    }
}