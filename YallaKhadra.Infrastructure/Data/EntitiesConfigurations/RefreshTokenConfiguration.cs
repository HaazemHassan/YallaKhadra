using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YallaKhadra.Core.Entities.IdentityEntities;

namespace YallaKhadra.Infrastructure.Data.EntitiesConfigurations
{
    internal class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.ToTable("RefreshToken", schema: "identity");

            builder.HasKey(rt => rt.Id);

            builder.Property(rt => rt.UserId)
                   .IsRequired();

            builder.Property(rt => rt.Token)
                   .IsRequired();

            builder.Property(rt => rt.AccessTokenJTI)
                   .IsRequired(false);

            builder.Property(rt => rt.Expires)
                   .IsRequired();

            builder.Property(rt => rt.Created)
                   .IsRequired();

            builder.Property(rt => rt.RevokationDate)
                   .IsRequired(false);

            builder.HasOne(rt => rt.User)
                   .WithMany(u => u.RefreshTokens)
                   .HasForeignKey(rt => rt.UserId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
