using Bolt.Domain.Entities;

public interface IReviewRepository
{
    Task<IEnumerable<Review>> GetByDriverIdAsync(Guid driverId, int page = 1, int pageSize = 10, CancellationToken cancellationToken = default);
    Task<Review?> GetByRideIdAsync(Guid rideId, CancellationToken cancellationToken = default);
    Task AddAsync(Review review, CancellationToken cancellationToken = default);
    Task<Review?> GetByDriverAndPassengerAsync(Guid driverId, Guid passengerId, CancellationToken cancellationToken = default);
}