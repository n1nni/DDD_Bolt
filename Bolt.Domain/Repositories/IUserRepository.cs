using Bolt.Domain.Entities;

namespace Bolt.Domain.Repositories;

public interface IUserRepository
{
    // Standard operations (automatically exclude deleted items)
    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Passenger?> GetPassengerByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Driver?> GetDriverByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<IEnumerable<Driver>> GetAvailableDriversAsync(CancellationToken cancellationToken = default);
    Task AddAsync(User user, CancellationToken cancellationToken = default);
    void Update(User user);

    // Soft delete operations
    void Remove(User user); // Marks as deleted (soft delete)
    void Restore(User user); // Restores from soft delete

    // Query all items (including deleted)
    Task<IEnumerable<User>> GetAllAsync(int page = 1, int pageSize = 20, CancellationToken cancellationToken = default);
    Task<IEnumerable<Driver>> GetAllDriversAsync(int page = 1, int pageSize = 20, CancellationToken cancellationToken = default);
    Task<IEnumerable<Passenger>> GetAllPassengersAsync(int page = 1, int pageSize = 20, CancellationToken cancellationToken = default);
}
