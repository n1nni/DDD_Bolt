using Bolt.Domain.Entities;
using Bolt.Domain.Repositories;

namespace Bolt.Domain.Repositories;

public interface IReviewRepository
{
    Task<Review?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Review>> GetByDriverIdAsync(Guid driverId, int page = 1, int pageSize = 20, CancellationToken cancellationToken = default);
    Task<IEnumerable<Review>> GetByPassengerIdAsync(Guid passengerId, int page = 1, int pageSize = 20, CancellationToken cancellationToken = default);
    Task<Review?> GetByRideIdAsync(Guid rideId, CancellationToken cancellationToken = default);
    Task AddAsync(Review review, CancellationToken cancellationToken = default);
    void Update(Review review);
    void Remove(Review review);
}
