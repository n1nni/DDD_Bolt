using Bolt.Domain.Entities;
using Bolt.Domain.Repositories;

namespace Bolt.Domain.Repositories;

public interface IRideOrderRepository
{
    // Standard operations (automatically exclude deleted items)
    Task<RideOrder?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<RideOrder?> GetByIdWithEventsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<RideOrder>> GetByPassengerIdAsync(Guid passengerId, int page = 1, int pageSize = 20, CancellationToken cancellationToken = default);
    Task<IEnumerable<RideOrder>> GetByDriverIdAsync(Guid driverId, int page = 1, int pageSize = 20, CancellationToken cancellationToken = default);
    Task<IEnumerable<RideOrder>> GetAvailableRidesAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<RideOrder>> GetActiveRidesByDriverAsync(Guid driverId, CancellationToken cancellationToken = default);
    Task AddAsync(RideOrder rideOrder, CancellationToken cancellationToken = default);
    void Update(RideOrder rideOrder);

    // Soft delete operations
    void Remove(RideOrder rideOrder); // Marks as deleted (soft delete)
    void Restore(RideOrder rideOrder); // Restores from soft delete

    // Query all items (including deleted)
    Task<IEnumerable<RideOrder>> GetAllAsync(int page = 1, int pageSize = 20, CancellationToken cancellationToken = default);
}