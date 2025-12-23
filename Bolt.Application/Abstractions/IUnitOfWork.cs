namespace Bolt.Application.Abstractions;

/// <summary>
/// Unit of Work pattern for coordinating repository operations and transactions.
/// </summary>
public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
