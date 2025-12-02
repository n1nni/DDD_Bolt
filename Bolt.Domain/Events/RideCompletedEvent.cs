namespace Bolt.Domain.Events;

public sealed class RideCompletedEvent : IDomainEvent
{
    public Guid RideOrderId { get; }
    public Guid DriverId { get; }
    public Guid UserId { get; }
    public DateTime OccurredOn { get; }

    public RideCompletedEvent(Guid rideOrderId, Guid driverId, Guid userId)
    {
        RideOrderId = rideOrderId;
        DriverId = driverId;
        UserId = userId;
        OccurredOn = DateTime.UtcNow;
    }
}
