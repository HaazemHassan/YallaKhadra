namespace YallaKhadra.Core.Entities.E_CommerceEntities {
    public class Product : BaseEntity<int> {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int PointsCost { get; set; }
        public int Stock { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int CategoryId { get; set; }

        public virtual ICollection<ProductImage> Images { get; set; } = new HashSet<ProductImage>();

        public virtual Category Category { get; set; } = null!;

        // Rich Domain Model Methods
        public bool HasSufficientStock(int quantity) {
            return Stock >= quantity;
        }

        public void ReduceStock(int quantity) {
            if (!HasSufficientStock(quantity))
                throw new InvalidOperationException($"Insufficient stock for product: {Name}");

            Stock -= quantity;
        }

        public void IncreaseStock(int quantity) {
            Stock += quantity;
        }

        public bool IsAvailable() {
            return IsActive && Stock > 0;
        }
    }
}
