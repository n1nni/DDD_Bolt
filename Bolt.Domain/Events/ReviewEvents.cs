namespace Bolt.Domain.Events;

public abstract class ReviewEvent : IDomainEvent
{
    public Guid ReviewId { get; }
    public DateTime OccurredOn { get; }
    public abstract string EventName { get; }

    protected ReviewEvent(Guid reviewId)
    {
        ReviewId = reviewId;
        OccurredOn = DateTime.UtcNow;
    }
}

public class ReviewCreatedEvent : ReviewEvent
{
    public Guid DriverId { get; }
    public Guid PassengerId { get; }
    public double Rating { get; }
    public override string EventName => "ReviewCreated";

    public ReviewCreatedEvent(Guid reviewId, Guid driverId, Guid passengerId, double rating)
        : base(reviewId)
    {
        DriverId = driverId;
        PassengerId = passengerId;
        Rating = rating;
    }
}
