namespace Bolt.Domain.Events;

// Marker interface for domain events.
public interface IDomainEvent
{
    DateTime OccurredOn { get; }
}
