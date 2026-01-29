using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YallaKhadra.Core.Entities.IdentityEntities;

namespace YallaKhadra.Infrastructure.Data.EntitiesConfigurations {
    internal class UserProfileImageConfiguration : IEntityTypeConfiguration<UserProfileImage> {
        public void Configure(EntityTypeBuilder<UserProfileImage> builder) {
            builder.ToTable("UserProfileImages", schema: "identity");

            builder.Property(upi => upi.UserId)
                   .IsRequired();

            builder.HasOne(upi => upi.User)
                   .WithOne(u => u.ProfileImage)
                   .HasForeignKey<ApplicationUser>(u => u.ProfileImageId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
