namespace Bolt.Domain.Events;

public sealed class RideStartedEvent : IDomainEvent
{
    public Guid RideId { get; }
    public Guid DriverId { get; }
    public DateTime OccurredOn { get; }

    public RideStartedEvent(Guid rideId, Guid driverId)
    {
        RideId = rideId;
        DriverId = driverId;
        OccurredOn = DateTime.UtcNow;
    }
}
