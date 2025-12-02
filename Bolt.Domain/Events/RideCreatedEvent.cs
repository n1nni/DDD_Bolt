using Bolt.Domain.ValueObjects;

namespace Bolt.Domain.Events;

public sealed class RideCreatedEvent : IDomainEvent
{
    public Guid RideOrderId { get; }
    public Guid UserId { get; }
    public Address Pickup { get; }
    public Address Destination { get; }
    public DateTime OccurredOn { get; }

    public RideCreatedEvent(Guid rideOrderId, Guid userId, Address pickup, Address destination)
    {
        RideOrderId = rideOrderId;
        UserId = userId;
        Pickup = pickup;
        Destination = destination;
        OccurredOn = DateTime.UtcNow;
    }
}
