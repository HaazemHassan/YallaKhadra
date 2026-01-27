using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YallaKhadra.Core.Entities.GreenEntities;

namespace YallaKhadra.Infrastructure.Data.EntitiesConfigurations
{
    public class CleanupImageConfiguration : IEntityTypeConfiguration<CleanupImage>
    {
        public void Configure(EntityTypeBuilder<CleanupImage> builder)
        {
            builder.ToTable("CleanupImages", schema: "green");

            builder.HasOne(x => x.CleanupTask)
                   .WithMany(t => t.Images)
                   .HasForeignKey(x => x.CleanupTaskId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
