using Bolt.Domain.ValueObjects;

namespace Bolt.Domain.Events;

public sealed class RideCompletedEvent : IDomainEvent
{
    public Guid RideId { get; }
    public Guid DriverId { get; }
    public Guid PassengerId { get; }
    public Money FinalFare { get; }
    public DateTime OccurredOn { get; }

    public RideCompletedEvent(Guid rideId, Guid driverId, Guid passengerId, Money finalFare)
    {
        RideId = rideId;
        DriverId = driverId;
        PassengerId = passengerId;
        FinalFare = finalFare;
        OccurredOn = DateTime.UtcNow;
    }
}
