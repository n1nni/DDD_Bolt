namespace Bolt.Domain.Events;

public sealed class RideAcceptedEvent : IDomainEvent
{
    public Guid RideId { get; }
    public Guid DriverId { get; }
    public DateTime OccurredOn { get; }

    public RideAcceptedEvent(Guid rideId, Guid driverId)
    {
        RideId = rideId;
        DriverId = driverId;
        OccurredOn = DateTime.UtcNow;
    }
}
