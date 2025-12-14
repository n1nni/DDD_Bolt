using Bolt.Domain.Entities;
public interface IReviewRepository
{
    // Standard operations (automatically exclude deleted items)
    Task<Review?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Review>> GetByDriverIdAsync(Guid driverId, int page = 1, int pageSize = 20, CancellationToken cancellationToken = default);
    Task<IEnumerable<Review>> GetByPassengerIdAsync(Guid passengerId, int page = 1, int pageSize = 20, CancellationToken cancellationToken = default);
    Task<Review?> GetByRideIdAsync(Guid rideId, CancellationToken cancellationToken = default);
    Task AddAsync(Review review, CancellationToken cancellationToken = default);
    void Update(Review review);

    // Soft delete operations
    void Remove(Review review); // Marks as deleted (soft delete)
    void Restore(Review review); // Restores from soft delete

    // Query all items (including deleted)
    Task<IEnumerable<Review>> GetAllAsync(int page = 1, int pageSize = 20, CancellationToken cancellationToken = default);
}