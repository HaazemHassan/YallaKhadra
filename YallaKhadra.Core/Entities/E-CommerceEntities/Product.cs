namespace YallaKhadra.Core.Entities.E_CommerceEntities
{
    public class Product : BaseEntity<int>
    {

        public virtual ICollection<ProductImage> Images { get; set; } = new HashSet<ProductImage>();
    }
}
