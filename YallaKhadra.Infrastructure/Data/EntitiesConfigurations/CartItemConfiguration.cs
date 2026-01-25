using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YallaKhadra.Core.Entities.E_CommerceEntities;

namespace YallaKhadra.Infrastructure.Data.EntitiesConfigurations {
    internal class CartItemConfiguration : IEntityTypeConfiguration<CartItem> {
        public void Configure(EntityTypeBuilder<CartItem> builder) {
            builder.ToTable("CartItems", schema: "ecommerce");

            builder.HasKey(ci => ci.Id);

            builder.Property(ci => ci.CartId)
                   .IsRequired();

            builder.Property(ci => ci.ProductId)
                   .IsRequired();

            builder.Property(ci => ci.Quantity)
                   .IsRequired()
                   .HasDefaultValue(1);

            builder.Property(ci => ci.PointsCost)
                   .IsRequired();

            builder.Property(ci => ci.AddedAt);

            builder.Property(ci => ci.IsSelected)
                   .IsRequired()
                   .HasDefaultValue(true);

            builder.HasOne(ci => ci.Cart)
                   .WithMany(c => c.Items)
                   .HasForeignKey(ci => ci.CartId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(ci => ci.Product)
                   .WithMany()
                   .HasForeignKey(ci => ci.ProductId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(ci => new { ci.CartId, ci.ProductId })
                   .IsUnique();
        }
    }
}
