using Bolt.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bolt.Persistence.Data.Configurations;

public class ReviewConfiguration : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> builder)
    {
        builder.HasKey(r => r.Id);

        builder.Property(r => r.RideId)
            .IsRequired();

        builder.Property(r => r.DriverId)
            .IsRequired();

        builder.Property(r => r.PassengerId)
            .IsRequired();

        builder.Property(r => r.Comment)
            .HasMaxLength(500);

        builder.Property(r => r.CreatedAt)
            .IsRequired();

        builder.HasIndex(r => r.RideId)
            .IsUnique();

        builder.HasIndex(r => r.DriverId);
        builder.HasIndex(r => r.PassengerId);
    }
}
