using Bolt.Domain.Entities;
using Bolt.Domain.Repositories;
using Bolt.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace Bolt.Persistence.Repositories;

public class ReviewRepository : IReviewRepository
{
    private readonly ApplicationDbContext _context;

    public ReviewRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    // Standard operations - automatically exclude deleted items via global query filter
    public async Task<Review?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Reviews
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Review>> GetByDriverIdAsync(
        Guid driverId,
        int page = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        return await _context.Reviews
            .Where(r => r.DriverId == driverId)
            .OrderByDescending(r => r.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Review>> GetByPassengerIdAsync(
        Guid passengerId,
        int page = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        return await _context.Reviews
            .Where(r => r.PassengerId == passengerId)
            .OrderByDescending(r => r.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<Review?> GetByRideIdAsync(Guid rideId, CancellationToken cancellationToken = default)
    {
        return await _context.Reviews
            .FirstOrDefaultAsync(r => r.RideId == rideId, cancellationToken);
    }

    public async Task AddAsync(Review review, CancellationToken cancellationToken = default)
    {
        await _context.Reviews.AddAsync(review, cancellationToken);
    }

    public void Update(Review review)
    {
        _context.Reviews.Update(review);
    }

    // Soft delete - EF Core will intercept and mark as deleted
    public void Remove(Review review)
    {
        _context.Reviews.Remove(review);
    }

    // Restore from soft delete
    public void Restore(Review review)
    {
        review.Restore();
        _context.Reviews.Update(review);
    }

    // Get all reviews (including deleted ones)
    public async Task<IEnumerable<Review>> GetAllAsync(
        int page = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        return await _context.Reviews
            .IgnoreQueryFilters()
            .OrderByDescending(r => r.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }
}