namespace YallaKhadra.Core.Entities.E_CommerceEntities {
    public class Category : BaseEntity<int> {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public virtual ICollection<Product> Products { get; set; } = new HashSet<Product>();
    }
}