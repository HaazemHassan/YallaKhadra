using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YallaKhadra.Core.Entities.E_CommerceEntities;

namespace YallaKhadra.Infrastructure.Data.EntitiesConfigurations {
    internal class OrderShippingDetailsConfiguration : IEntityTypeConfiguration<OrderShippingDetails> {
        public void Configure(EntityTypeBuilder<OrderShippingDetails> builder) {
            builder.ToTable("OrderShippingDetails", schema: "ecommerce");

            builder.HasKey(sd => sd.Id);

            builder.Property(sd => sd.OrderId)
                   .IsRequired();

            builder.Property(sd => sd.FullName)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(sd => sd.PhoneNumber)
                   .IsRequired()
                   .HasMaxLength(20);

            builder.Property(sd => sd.City)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.Property(sd => sd.StreetAddress)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(sd => sd.BuildingNumber)
                   .HasMaxLength(20);

            builder.Property(sd => sd.Landmark)
                   .HasMaxLength(100);

            builder.Property(sd => sd.ShippingNotes)
                   .HasMaxLength(500);

            builder.HasOne(sd => sd.Order)
                   .WithOne(o => o.ShippingDetails)
                   .HasForeignKey<OrderShippingDetails>(sd => sd.OrderId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(sd => sd.OrderId)
                   .IsUnique();
        }
    }
}
