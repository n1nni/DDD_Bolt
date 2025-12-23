using Bolt.Domain.Entities;

namespace Bolt.Domain.Repositories;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Passenger?> GetPassengerByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Driver?> GetDriverByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<IEnumerable<Driver>> GetAvailableDriversAsync(CancellationToken cancellationToken = default);
    Task AddAsync(User user, CancellationToken cancellationToken = default);
    void Update(User user);
    Task<IEnumerable<User>> GetAllAsync(int page = 1, int pageSize = 10, CancellationToken cancellationToken = default);
    Task<IEnumerable<Driver>> GetAllDriversAsync(int page = 1, int pageSize = 10, CancellationToken cancellationToken = default);
    Task<IEnumerable<Passenger>> GetAllPassengersAsync(int page = 1, int pageSize = 10, CancellationToken cancellationToken = default);
}
