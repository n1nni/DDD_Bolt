using Bolt.Domain.Entities;

namespace Bolt.Domain.Repositories;

public interface IRideOrderRepository
{
    Task<RideOrder?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<RideOrder>> GetByDriverIdAsync(Guid driverId, int page = 1, int pageSize = 10, CancellationToken cancellationToken = default);
    Task<IEnumerable<RideOrder>> GetAvailableRidesAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<RideOrder>> GetActiveRidesByDriverAsync(Guid driverId, CancellationToken cancellationToken = default);
    Task AddAsync(RideOrder rideOrder, CancellationToken cancellationToken = default);
    void Update(RideOrder rideOrder);
    Task<IEnumerable<RideOrder>> GetAllAsync(int page = 1, int pageSize = 10, CancellationToken cancellationToken = default);
}