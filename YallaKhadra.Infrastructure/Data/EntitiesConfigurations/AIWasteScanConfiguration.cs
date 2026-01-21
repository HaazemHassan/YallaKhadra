using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YallaKhadra.Core.Entities;

namespace YallaKhadra.Infrastructure.Data.EntitiesConfigurations
{
    public class AIWasteScanConfiguration : IEntityTypeConfiguration<AIWasteScan>
    {
        public void Configure(EntityTypeBuilder<AIWasteScan> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.AIPredictedType)
                .HasMaxLength(100)
                .IsRequired(false);

            builder.Property(e => e.AIExplanation)
                .HasMaxLength(1000)
                .IsRequired(false);

            builder.HasOne(e => e.User)
                .WithMany() 
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
