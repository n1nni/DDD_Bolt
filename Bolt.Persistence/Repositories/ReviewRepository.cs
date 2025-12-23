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

    public async Task<IEnumerable<Review>> GetByDriverIdAsync(
        Guid driverId,
        int page = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        return await _context.Reviews
            .Where(r => r.DriverId == driverId)
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

}