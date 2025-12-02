using Bolt.Domain.Entities;

namespace Bolt.Domain.Interfaces;

public interface IRideOrderRepository : IRepository<RideOrder>
{
    Task<IEnumerable<RideOrder>> GetByUserIdAsync(Guid userId, int page = 1, int pageSize = 20);
    Task<IEnumerable<RideOrder>> GetAvailableOrdersAsync(); // for drivers
    Task<RideOrder?> GetByIdAsync(Guid id, bool includeEvents = false);
}
