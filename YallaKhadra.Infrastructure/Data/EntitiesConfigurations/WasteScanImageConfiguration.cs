using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YallaKhadra.Core.Entities;

namespace YallaKhadra.Infrastructure.Data.EntitiesConfigurations
{
    public class WasteScanImageConfiguration : IEntityTypeConfiguration<WasteScanImage>
    {
        public void Configure(EntityTypeBuilder<WasteScanImage> builder)
        {
            builder.ToTable("WasteScanImages");

            builder.HasOne(i => i.AIWasteScan)
                .WithOne(s => s.WasteScanImage)
                .HasForeignKey<WasteScanImage>(i => i.AIWasteScanId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(i => i.AIWasteScanId).IsUnique();
        }
    }
}
