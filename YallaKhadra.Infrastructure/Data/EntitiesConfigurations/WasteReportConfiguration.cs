using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YallaKhadra.Core.Entities;

namespace YallaKhadra.Infrastructure.Data.EntitiesConfigurations
{
    internal class WasteReportConfiguration : IEntityTypeConfiguration<WasteReport>
    {
        public void Configure(EntityTypeBuilder<WasteReport> builder)
        {
            builder.Property(e => e.Latitude)
               .HasPrecision(9, 6)
               .IsRequired();

            builder.Property(e => e.Longitude)
                   .HasPrecision(9, 6)
                   .IsRequired();

            builder.HasMany(w => w.Images)
                   .WithOne(i => i.Report)
                   .HasForeignKey(i => i.ReportId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
