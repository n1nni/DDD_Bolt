using Bolt.Application.Abstractions;
using Bolt.Domain.Abstractions;
using Bolt.Domain.Entities;
using Bolt.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Bolt.Persistence.Data;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<RideOrder> RideOrders { get; set; }
    public DbSet<Review> Reviews { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        // Configure TPH inheritance
        modelBuilder.Entity<User>()
            .ToTable("Users")
            .HasDiscriminator<string>("UserRole")
            .HasValue<Driver>("Driver")
            .HasValue<Passenger>("Passenger");

        // Configure RideOrder with owned types - USE THIS EXACT CONFIGURATION
        modelBuilder.Entity<RideOrder>(builder =>
        {
            // Configure PickupAddress
            builder.OwnsOne(r => r.PickupAddress, addressBuilder =>
            {
                addressBuilder.Property(a => a.Street).IsRequired().HasMaxLength(200);
                addressBuilder.Property(a => a.City).IsRequired().HasMaxLength(100);
                addressBuilder.Property(a => a.PostalCode).HasMaxLength(20);

                // Configure nested Location
                addressBuilder.OwnsOne(a => a.Location, locationBuilder =>
                {
                    locationBuilder.Property(l => l.Latitude).IsRequired().HasColumnType("decimal(9,6)");
                    locationBuilder.Property(l => l.Longitude).IsRequired().HasColumnType("decimal(9,6)");
                });
            });

            // Configure DestinationAddress
            builder.OwnsOne(r => r.DestinationAddress, addressBuilder =>
            {
                addressBuilder.Property(a => a.Street).IsRequired().HasMaxLength(200);
                addressBuilder.Property(a => a.City).IsRequired().HasMaxLength(100);
                addressBuilder.Property(a => a.PostalCode).HasMaxLength(20);

                // Configure nested Location
                addressBuilder.OwnsOne(a => a.Location, locationBuilder =>
                {
                    locationBuilder.Property(l => l.Latitude).IsRequired().HasColumnType("decimal(9,6)");
                    locationBuilder.Property(l => l.Longitude).IsRequired().HasColumnType("decimal(9,6)");
                });
            });

            // Configure Money value objects
            builder.OwnsOne(r => r.EstimatedFare, moneyBuilder =>
            {
                moneyBuilder.Property(m => m.Amount).IsRequired().HasColumnType("decimal(18,2)");
                moneyBuilder.Property(m => m.Currency).IsRequired().HasMaxLength(3);
            });

            builder.OwnsOne(r => r.FinalFare, moneyBuilder =>
            {
                moneyBuilder.Property(m => m.Amount).HasColumnType("decimal(18,2)");
                moneyBuilder.Property(m => m.Currency).HasMaxLength(3);
            });
        });

        modelBuilder.Entity<Review>(builder =>
        {
            builder.OwnsOne(r => r.Rating, ratingBuilder =>
            {
                ratingBuilder.Property(r => r.Value).IsRequired().HasColumnType("decimal(3,1)");
                ratingBuilder.Property(r => r.TotalReviews).IsRequired();
            });
        });

        // Configure domain events
        modelBuilder.Entity<RideOrder>()
            .Ignore(r => r.DomainEvents);

        // Apply global query filter for soft delete
        modelBuilder.Entity<User>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<RideOrder>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Review>().HasQueryFilter(e => !e.IsDeleted);
    }

    // Override SaveChanges to handle soft delete
    public override int SaveChanges()
    {
        HandleSoftDelete();
        return base.SaveChanges();
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        HandleSoftDelete();
        return await base.SaveChangesAsync(cancellationToken);
    }

    private void HandleSoftDelete()
    {
        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.Entity is ISoftDelete softDeleteEntity && entry.State == EntityState.Deleted)
            {
                // Change state to modified instead of deleted
                entry.State = EntityState.Modified;
                // Mark as deleted
                softDeleteEntity.MarkAsDeleted();
            }
        }
    }
}
