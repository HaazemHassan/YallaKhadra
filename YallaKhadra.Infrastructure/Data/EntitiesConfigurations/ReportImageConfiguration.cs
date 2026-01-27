using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YallaKhadra.Core.Entities.GreenEntities;

namespace YallaKhadra.Infrastructure.Data.EntitiesConfigurations
{
    public class ReportImageConfiguration : IEntityTypeConfiguration<ReportImage>
    {
        public void Configure(EntityTypeBuilder<ReportImage> builder)
        {
            builder.ToTable("ReportImages", schema: "green");

            builder.HasOne(x => x.Report)
                   .WithMany(w => w.Images) 
                   .HasForeignKey(x => x.ReportId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
