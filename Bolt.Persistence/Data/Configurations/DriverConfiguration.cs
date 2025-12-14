using Bolt.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bolt.Persistence.Data.Configurations;

public class DriverConfiguration : IEntityTypeConfiguration<Driver>
{
    public void Configure(EntityTypeBuilder<Driver> builder)
    {
        builder.Property(d => d.LicenseNumber)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(d => d.VehicleModel)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(d => d.VehiclePlateNumber)
            .HasMaxLength(20)
            .IsRequired()
            .HasConversion(
                v => v.ToUpperInvariant(),
                v => v);

        builder.Property(d => d.IsAvailable)
            .IsRequired();

        builder.OwnsOne(d => d.Rating);

        // Configure completed rides as JSON array for simplicity
        // Alternatively, you could create a separate table
        builder.Property(d => d.CompletedRideIds)
            .HasConversion(
                v => string.Join(',', v),
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries)
                      .Select(Guid.Parse).ToList())
            .HasColumnName("CompletedRideIds");
    }
}
