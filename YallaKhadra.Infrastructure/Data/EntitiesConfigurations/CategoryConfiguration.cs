using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YallaKhadra.Core.Entities.E_CommerceEntities;

namespace YallaKhadra.Infrastructure.Data.EntitiesConfigurations {
    internal class CategoryConfiguration : IEntityTypeConfiguration<Category> {
        public void Configure(EntityTypeBuilder<Category> builder) {
            builder.ToTable("Categories", schema: "ecommerce");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.Name)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.Property(c => c.Description)
                   .IsRequired()
                   .HasMaxLength(250);

            builder.HasMany(c => c.Products)
                   .WithOne(p => p.Category)
                   .HasForeignKey(p => p.CategoryId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(c => c.Name)
                   .IsUnique();
        }
    }
}
