namespace Bolt.Application.Abstractions;

/// <summary>
/// Abstraction for the application database context.
/// Used by application layer to access EF Core features without direct dependency.
/// </summary>
public interface IApplicationDbContext
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
