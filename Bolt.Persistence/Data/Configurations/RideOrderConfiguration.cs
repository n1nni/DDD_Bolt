using Bolt.Domain.Entities;
using Bolt.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Bolt.Persistence.Data.Configurations;

public class RideOrderConfiguration : IEntityTypeConfiguration<RideOrder>
{
    public void Configure(EntityTypeBuilder<RideOrder> builder)
    {
        builder.HasKey(r => r.Id);

        builder.Property(r => r.PassengerId)
            .IsRequired();

        builder.Property(r => r.DriverId);

        builder.Property(r => r.Status)
            .HasConversion(new EnumToStringConverter<RideStatus>())
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(r => r.CreatedAt)
            .IsRequired();

        builder.Property(r => r.AcceptedAt);
        builder.Property(r => r.StartedAt);
        builder.Property(r => r.CompletedAt);
        builder.Property(r => r.CancelledAt);

        builder.Property(r => r.CancellationReason)
            .HasMaxLength(500);

        builder.Property(r => r.CancelledBy);

        builder.HasIndex(r => r.PassengerId);
        builder.HasIndex(r => r.DriverId);
        builder.HasIndex(r => r.Status);
    }
}
