using Bolt.Domain.Entities;
using Bolt.Domain.Repositories;

namespace Bolt.Domain.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<Passenger?> GetPassengerByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<Driver?> GetDriverByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
        Task<IEnumerable<Driver>> GetAvailableDriversAsync(CancellationToken cancellationToken = default);
        Task AddAsync(User user, CancellationToken cancellationToken = default);
        void Update(User user);
        void Remove(User user);
    }
}
