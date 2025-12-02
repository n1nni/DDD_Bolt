using Bolt.Domain.Entities;

namespace Bolt.Domain.Interfaces;

public interface IDriverRepository : IRepository<Driver>
{
    Task<Driver?> GetByUserIdAsync(Guid userId);
}
