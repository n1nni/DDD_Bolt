using Bolt.Domain.ValueObjects;

namespace Bolt.Domain.Events;

public sealed class RideCreatedEvent : IDomainEvent
{
    public Guid RideId { get; }
    public Guid PassengerId { get; }
    public Address Pickup { get; }
    public Address Destination { get; }
    public DateTime OccurredOn { get; }

    public RideCreatedEvent(Guid rideId, Guid passengerId, Address pickup, Address destination)
    {
        RideId = rideId;
        PassengerId = passengerId;
        Pickup = pickup;
        Destination = destination;
        OccurredOn = DateTime.UtcNow;
    }
}
