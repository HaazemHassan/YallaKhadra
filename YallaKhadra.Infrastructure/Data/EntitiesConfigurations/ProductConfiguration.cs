using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YallaKhadra.Core.Entities.E_CommerceEntities;

namespace YallaKhadra.Infrastructure.Data.EntitiesConfigurations {
    internal class ProductConfiguration : IEntityTypeConfiguration<Product> {
        public void Configure(EntityTypeBuilder<Product> builder) {
            builder.ToTable("Products", schema: "ecommerce");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Name)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.Property(p => p.Description)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(p => p.PointsCost)
                   .IsRequired();


            builder.Property(p => p.Stock)
                   .IsRequired();

            builder.Property(p => p.IsActive)
                   .IsRequired();

            builder.Property(p => p.CreatedAt)
                   .IsRequired();

            builder.Property(p => p.CategoryId)
                   .IsRequired(true);

            builder.HasOne(p => p.Category)
                   .WithMany(c => c.Products)
                   .HasForeignKey(p => p.CategoryId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(p => p.Images)
                   .WithOne(pi => pi.Product)
                   .HasForeignKey(pi => pi.ProductId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
