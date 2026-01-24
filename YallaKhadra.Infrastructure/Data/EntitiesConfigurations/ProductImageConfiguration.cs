using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YallaKhadra.Core.Entities.E_CommerceEntities;

namespace YallaKhadra.Infrastructure.Data.EntitiesConfigurations {
    internal class ProductImageConfiguration : IEntityTypeConfiguration<ProductImage> {
        public void Configure(EntityTypeBuilder<ProductImage> builder) {
            builder.ToTable("ProductImage", schema: "ecommerce");

            builder.Property(pi => pi.IsMain)
                   .IsRequired()
                   .HasDefaultValue(false);

            builder.Property(pi => pi.ProductId)
                   .IsRequired();

            builder.HasOne(pi => pi.Product)
                   .WithMany(p => p.Images)
                   .HasForeignKey(pi => pi.ProductId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
