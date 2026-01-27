using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YallaKhadra.Core.Entities.IdentityEntities;

namespace YallaKhadra.Infrastructure.Data.EntitiesConfigurations
{
    internal class ApplicationRoleConfiguration : IEntityTypeConfiguration<ApplicationRole>
    {
        public void Configure(EntityTypeBuilder<ApplicationRole> builder)
        {
            builder.ToTable("AspNetRoles", schema: "identity");
        }
    }
}
