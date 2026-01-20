using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YallaKhadra.Core.Entities;

namespace YallaKhadra.Infrastructure.Data.EntitiesConfigurations
{
    public class CleanupTaskConfiguration : IEntityTypeConfiguration<CleanupTask>
    {
        public void Configure(EntityTypeBuilder<CleanupTask> builder)
        {
            // Relations
            builder.HasOne(x => x.Worker)
                   .WithMany()
                   .HasForeignKey(x => x.WorkerId)
                   .OnDelete(DeleteBehavior.Restrict);


            builder.HasOne(x => x.Report)
                   .WithMany()
                   .HasForeignKey(x => x.ReportId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.Images)
                   .WithOne(x => x.CleanupTask)
                   .HasForeignKey(x => x.CleanupTaskId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
