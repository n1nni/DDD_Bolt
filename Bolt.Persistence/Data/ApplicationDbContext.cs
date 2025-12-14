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

        // Simple owned type configuration - NO nested configuration needed
        modelBuilder.Entity<RideOrder>(builder =>
        {
            builder.OwnsOne(r => r.PickupAddress);
            builder.OwnsOne(r => r.DestinationAddress);
            builder.OwnsOne(r => r.EstimatedFare);
            builder.OwnsOne(r => r.FinalFare);
        });

        modelBuilder.Entity<Review>(builder =>
        {
            builder.OwnsOne(r => r.Rating);
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
