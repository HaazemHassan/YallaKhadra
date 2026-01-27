using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YallaKhadra.Core.Entities.PointsEntities;

namespace YallaKhadra.Infrastructure.Data.EntitiesConfigurations {
    internal class PointsTransactionConfiguration : IEntityTypeConfiguration<PointsTransaction> {
        public void Configure(EntityTypeBuilder<PointsTransaction> builder) {
            builder.ToTable("PointsTransactions", schema: "points");

            builder.HasKey(pt => pt.Id);

            builder.Property(pt => pt.Points)
                   .IsRequired();

            builder.Property(pt => pt.TransactionType)
                   .IsRequired();

            builder.Property(pt => pt.pointsSource)
                   .IsRequired();

            builder.Property(pt => pt.SourceId)
                   .IsRequired();

            builder.Property(pt => pt.CreatedAt)
                   .IsRequired();

            builder.Property(pt => pt.UserId)
                   .IsRequired();

            builder.HasOne(pt => pt.User)
                   .WithMany(u => u.PointsTransactions)
                   .HasForeignKey(pt => pt.UserId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
