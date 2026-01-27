using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YallaKhadra.Core.Entities.IdentityEntities;

namespace YallaKhadra.Infrastructure.Data.EntitiesConfigurations
{
    internal class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.ToTable("AspNetUsers", schema: "identity");

            builder.Property(u => u.FirstName)
                   .IsRequired();

            builder.Property(u => u.LastName)
                   .IsRequired();

            builder.Property(u => u.Address)
                   .IsRequired(false);

            builder.Property(u => u.PointsBalance)
                   .IsRequired()
                   .HasDefaultValue(0);

            builder.Property(u => u.CreatedAt)
                   .IsRequired();

            builder.HasMany(u => u.RefreshTokens)
                   .WithOne(rt => rt.User)
                   .HasForeignKey(rt => rt.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(u => u.PointsTransactions)
                   .WithOne(pt => pt.User)
                   .HasForeignKey(pt => pt.UserId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
