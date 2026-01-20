using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YallaKhadra.Core.Entities;

namespace YallaKhadra.Infrastructure.Data.EntitiesConfigurations
{
    public class CleanupImageConfiguration : IEntityTypeConfiguration<CleanupImage>
    {
        public void Configure(EntityTypeBuilder<CleanupImage> builder)
        {
            builder.ToTable("CleanupImages");

            builder.HasOne(x => x.CleanupTask)
                   .WithMany(t => t.Images)
                   .HasForeignKey(x => x.CleanupTaskId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
