using YallaKhadra.Core.Entities.BaseEntities;
using YallaKhadra.Core.Entities.IdentityEntities;

namespace YallaKhadra.Core.Entities.E_CommerceEntities {
    public class Cart : BaseEntity<int> {
        public int UserId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public virtual ApplicationUser User { get; set; } = null!;
        public virtual ICollection<CartItem> Items { get; set; } = new HashSet<CartItem>();

        public void AddOrUpdateItem(Product product, int quantity) {
            var existingItem = Items.FirstOrDefault(ci => ci.ProductId == product.Id);

            if (existingItem is not null) {
                existingItem.Quantity += quantity;
                existingItem.PointsCost = product.PointsCost;
            }
            else {
                Items.Add(new CartItem {
                    ProductId = product.Id,
                    Quantity = quantity,
                    PointsCost = product.PointsCost,
                    AddedAt = DateTime.UtcNow,
                    IsSelected = true
                });
            }

            UpdatedAt = DateTime.UtcNow;
        }

        public void RemoveItem(int cartItemId) {
            var item = Items.FirstOrDefault(i => i.Id == cartItemId);
            if (item is not null) {
                Items.Remove(item);
                UpdatedAt = DateTime.UtcNow;
            }
        }

        public void ClearItems() {
            Items.Clear();
            UpdatedAt = DateTime.UtcNow;
        }

        public bool IsOwnedBy(int userId) {
            return UserId == userId;
        }

        public void MarkAsUpdated() {
            UpdatedAt = DateTime.UtcNow;
        }

        public bool IsProductExists(int productId) {
            return Items.Any(i => i.ProductId == productId);
        }
    }
}