using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YallaKhadra.Core.Entities.IdentityEntities;

namespace YallaKhadra.Infrastructure.Data.EntitiesConfigurations {
    internal class VerificationCodeConfiguration : IEntityTypeConfiguration<VerificationCode> {
        public void Configure(EntityTypeBuilder<VerificationCode> builder) {
            builder.ToTable("VerificationCodes", schema: "identity");

            builder.HasKey(vc => vc.Id);

            builder.Property(vc => vc.ApplicationUserId)
                .IsRequired();

            builder.Property(vc => vc.Code)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(vc => vc.Type)
                .IsRequired();

            builder.Property(vc => vc.Status)
                .IsRequired();

            builder.Property(vc => vc.ExpiresAt)
                .IsRequired();

            builder.Property(vc => vc.Attempts)
                .IsRequired()
                .HasDefaultValue(0);

            builder.Property(vc => vc.CreatedAt)
                .IsRequired();

            builder.HasIndex(vc => new { vc.ApplicationUserId, vc.Type, vc.Status });

            builder.HasOne(vc => vc.ApplicationUser)
                .WithMany(u => u.VerificationCodes)
                .HasForeignKey(vc => vc.ApplicationUserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
