namespace Bolt.Domain.Events;

public sealed class RideCancelledEvent : IDomainEvent
{
    public Guid RideId { get; }
    public Guid CancelledBy { get; }
    public string Reason { get; }
    public DateTime OccurredOn { get; }

    public RideCancelledEvent(Guid rideId, Guid cancelledBy, string reason)
    {
        RideId = rideId;
        CancelledBy = cancelledBy;
        Reason = reason;
        OccurredOn = DateTime.UtcNow;
    }
}
