using Bolt.Domain.Events;

namespace Bolt.Domain.Abstractions;

/// Marker interface for aggregate roots.
public interface IAggregateRoot 
{
    IReadOnlyCollection<IDomainEvent> DomainEvents { get; }
    void ClearDomainEvents();
}
