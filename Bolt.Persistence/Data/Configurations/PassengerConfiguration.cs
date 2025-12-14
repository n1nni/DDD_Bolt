using Bolt.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bolt.Persistence.Data.Configurations
{
    public class PassengerConfiguration : IEntityTypeConfiguration<Passenger>
    {
        public void Configure(EntityTypeBuilder<Passenger> builder)
        {
            builder.OwnsOne(p => p.Rating);

            builder.Property(p => p.PreferredPaymentMethod)
                .HasMaxLength(50);

            // Configure ride history as JSON array
            builder.Property(p => p.RideHistoryIds)
                .HasConversion(
                    v => string.Join(',', v),
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries)
                          .Select(Guid.Parse).ToList())
                .HasColumnName("RideHistoryIds");
        }
    }
}
