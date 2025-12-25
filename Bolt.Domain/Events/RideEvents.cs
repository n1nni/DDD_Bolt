namespace Bolt.Domain.Events;

public abstract class RideEvent : IDomainEvent
{
    public Guid RideId { get; }
    public DateTime OccurredOn { get; }
    public abstract string EventName { get; }

    protected RideEvent(Guid rideId)
    {
        RideId = rideId;
        OccurredOn = DateTime.UtcNow;
    }
}

public class RideCreatedEvent : RideEvent
{
    public Guid PassengerId { get; }
    public override string EventName => "RideCreated";

    public RideCreatedEvent(Guid rideId, Guid passengerId)
        : base(rideId)
    {
        PassengerId = passengerId;
    }
}

public class RideAcceptedEvent : RideEvent
{
    public Guid DriverId { get; }
    public override string EventName => "RideAccepted";

    public RideAcceptedEvent(Guid rideId, Guid driverId)
        : base(rideId)
    {
        DriverId = driverId;
    }
}

public class RideStartedEvent : RideEvent
{
    public override string EventName => "RideStarted";

    public RideStartedEvent(Guid rideId) : base(rideId) { }
}

public class RideCompletedEvent : RideEvent
{
    public decimal FinalFare { get; }
    public override string EventName => "RideCompleted";

    public RideCompletedEvent(Guid rideId, decimal finalFare)
        : base(rideId)
    {
        FinalFare = finalFare;
    }
}

public class RideCancelledEvent : RideEvent
{
    public Guid CancelledBy { get; }
    public string Reason { get; }
    public override string EventName => "RideCancelled";

    public RideCancelledEvent(Guid rideId, Guid cancelledBy, string reason)
        : base(rideId)
    {
        CancelledBy = cancelledBy;
        Reason = reason;
    }
}
