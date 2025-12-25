using Bolt.Domain.Entities;
using Bolt.Domain.Repositories;
using Bolt.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace Bolt.Persistence.Repositories;

public class RideOrderRepository : IRideOrderRepository
{
    private readonly ApplicationDbContext _context;

    public RideOrderRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<RideOrder?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.RideOrders
        .Include(r => r.PickupAddress)
        .Include(r => r.DestinationAddress)
        .Include(r => r.EstimatedFare)
        .Include(r => r.FinalFare)
        .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<RideOrder>> GetByPassengerIdAsync(
        Guid passengerId,
        int page = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        return await _context.RideOrders
        .Include(r => r.PickupAddress)
        .Include(r => r.DestinationAddress)
        .Include(r => r.EstimatedFare)
        .Include(r => r.FinalFare)
        .Where(r => r.PassengerId == passengerId)
        .OrderByDescending(r => r.CreatedAt)
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<RideOrder>> GetAvailableRidesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.RideOrders
        .Include(r => r.PickupAddress)
        .Include(r => r.DestinationAddress)
        .Include(r => r.EstimatedFare)
        .Include(r => r.FinalFare)
        .Where(r => r.Status == Domain.Enums.RideStatus.Created)
        .OrderBy(r => r.CreatedAt)
        .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<RideOrder>> GetActiveRidesByDriverAsync(
        Guid driverId,
        CancellationToken cancellationToken = default)
    {
        return await _context.RideOrders
            .Where(r => r.DriverId == driverId &&
                       (r.Status == Domain.Enums.RideStatus.Accepted ||
                        r.Status == Domain.Enums.RideStatus.DriverArriving ||
                        r.Status == Domain.Enums.RideStatus.InProgress))
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(RideOrder rideOrder, CancellationToken cancellationToken = default)
    {
        await _context.RideOrders.AddAsync(rideOrder, cancellationToken);
    }

    public void Update(RideOrder rideOrder)
    {
        _context.RideOrders.Update(rideOrder);
    }

    // Get all rides 
    public async Task<IEnumerable<RideOrder>> GetAllAsync(
        int page = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        return await _context.RideOrders
        .IgnoreQueryFilters()
        .Include(r => r.PickupAddress)
        .Include(r => r.DestinationAddress)
        .Include(r => r.EstimatedFare)
        .Include(r => r.FinalFare)
        .OrderByDescending(r => r.CreatedAt)
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync(cancellationToken);
    }

}