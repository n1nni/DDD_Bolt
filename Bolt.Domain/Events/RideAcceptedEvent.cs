namespace Bolt.Domain.Events;

public sealed class RideAcceptedEvent : IDomainEvent
{
    public Guid RideOrderId { get; }
    public Guid DriverId { get; }
    public DateTime OccurredOn { get; }

    public RideAcceptedEvent(Guid rideOrderId, Guid driverId)
    {
        RideOrderId = rideOrderId;
        DriverId = driverId;
        OccurredOn = DateTime.UtcNow;
    }
}
