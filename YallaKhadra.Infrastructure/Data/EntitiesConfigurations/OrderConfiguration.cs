using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YallaKhadra.Core.Entities.E_CommerceEntities;

namespace YallaKhadra.Infrastructure.Data.EntitiesConfigurations {
    internal class OrderConfiguration : IEntityTypeConfiguration<Order> {
        public void Configure(EntityTypeBuilder<Order> builder) {
            builder.ToTable("Orders", schema: "ecommerce");

            builder.HasKey(o => o.Id);

            builder.Property(o => o.UserId)
                   .IsRequired();

            builder.Property(o => o.OrderDate)
                   .IsRequired();

            builder.Property(o => o.TotalPoints)
                   .IsRequired();

            builder.Property(o => o.Status)
                   .IsRequired();

            builder.Property(o => o.ShippingDetailsId)
                   .IsRequired();

            builder.HasOne(o => o.ApplicationUser)
                   .WithMany(u => u.Orders)
                   .HasForeignKey(o => o.UserId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(o => o.ShippingDetails)
                   .WithOne(sd => sd.Order)
                   .HasForeignKey<Order>(o => o.ShippingDetailsId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(o => o.OrderItems)
                   .WithOne(oi => oi.Order)
                   .HasForeignKey(oi => oi.OrderId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(o => o.UserId);
            builder.HasIndex(o => o.OrderDate);
            builder.HasIndex(o => o.Status);
            builder.HasIndex(o => o.ShippingDetailsId)
                   .IsUnique();
        }
    }
}
