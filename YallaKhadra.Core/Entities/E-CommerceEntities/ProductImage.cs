namespace YallaKhadra.Core.Entities.E_CommerceEntities
{
    public class ProductImage : BaseImage
    {
        public bool IsMain { get; set; } = false;
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;
    }
}
