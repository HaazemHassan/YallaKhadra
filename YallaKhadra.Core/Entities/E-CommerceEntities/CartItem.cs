using YallaKhadra.Core.Entities.BaseEntities;

namespace YallaKhadra.Core.Entities.E_CommerceEntities {
    public class CartItem : BaseEntity<int> {
        public int CartId { get; set; }
        public virtual Cart Cart { get; set; } = null!;

        public int ProductId { get; set; }
        public virtual Product Product { get; set; } = null!;

        public int Quantity { get; set; }

        public int PointsCost { get; set; }

        public DateTime AddedAt { get; set; } = DateTime.UtcNow;

        public bool IsSelected { get; set; } = true;

        public void UpdateQuantity(int quantity, int productStock) {
            if (quantity <= 0)
                throw new InvalidOperationException("Quantity must be greater than 0.");

            if (productStock < quantity)
                throw new InvalidOperationException("Insufficient stock.");

            Quantity = quantity;
        }

        public void ToggleSelection(bool isSelected) {
            IsSelected = isSelected;
        }

        public int CalculateTotalPoints() {
            return Quantity * PointsCost;
        }

        public bool HasPriceChanged(int currentProductPrice) {
            return PointsCost != currentProductPrice;
        }

        public void SyncPrice(int currentProductPrice) {
            PointsCost = currentProductPrice;
        }

        public int CalculateTotalPointsWithCurrentPrice(int currentProductPrice) {
            return Quantity * currentProductPrice;
        }
    }
}