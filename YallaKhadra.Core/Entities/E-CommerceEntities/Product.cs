namespace YallaKhadra.Core.Entities.E_CommerceEntities {
    public class Product : BaseEntity<int> {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int PointsCost { get; set; }
        public int Stock { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public virtual ICollection<ProductImage> Images { get; set; } = new HashSet<ProductImage>();
    }
}
