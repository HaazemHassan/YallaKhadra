using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YallaKhadra.Core.Entities;

namespace YallaKhadra.Infrastructure.Data.EntitiesConfigurations
{
    public class BaseImageConfiguration : IEntityTypeConfiguration<BaseImage>
    {
        public void Configure(EntityTypeBuilder<BaseImage> builder)
        {
            builder.UseTptMappingStrategy();

            builder.Property(e => e.Url).IsRequired();
            builder.Property(e => e.PublicId).IsRequired();
        }
    }
}
