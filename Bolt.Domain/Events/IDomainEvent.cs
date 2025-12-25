namespace Bolt.Domain.Events;

public interface IDomainEvent
{
    DateTime OccurredOn { get; }
    string EventName { get; }
}
