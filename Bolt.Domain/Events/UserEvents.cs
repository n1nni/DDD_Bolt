namespace Bolt.Domain.Events;

public abstract class UserEvent : IDomainEvent
{
    public Guid UserId { get; }
    public DateTime OccurredOn { get; }
    public abstract string EventName { get; }

    protected UserEvent(Guid userId)
    {
        UserId = userId;
        OccurredOn = DateTime.UtcNow;
    }
}

public class UserCreatedEvent : UserEvent
{
    public string Role { get; }
    public override string EventName => "UserCreated";

    public UserCreatedEvent(Guid userId, string role)
        : base(userId)
    {
        Role = role;
    }
}

public class DriverAvailabilityChangedEvent : UserEvent
{
    public bool IsAvailable { get; }
    public override string EventName => "DriverAvailabilityChanged";

    public DriverAvailabilityChangedEvent(Guid userId, bool isAvailable)
        : base(userId)
    {
        IsAvailable = isAvailable;
    }
}
