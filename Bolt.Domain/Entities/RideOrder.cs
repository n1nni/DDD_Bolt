using Bolt.Domain.Abstractions;
using Bolt.Domain.Enums;
using Bolt.Domain.Events;
using Bolt.Domain.Exceptions;
using Bolt.Domain.Shared;
using Bolt.Domain.ValueObjects;

namespace Bolt.Domain.Entities;

public class RideOrder : IAggregateRoot
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public Guid? DriverId { get; private set; }
    public Address Pickup { get; private set; }
    public Address Destination { get; private set; }
    public Money EstimatedFare { get; private set; }
    public Money? FinalFare { get; private set; }
    public OrderStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? AcceptedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public string? CancellationReason { get; private set; }

    private readonly List<IDomainEvent> _domainEvents = new();
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    private RideOrder() { } // EF / ORM

    private RideOrder(Guid id, Guid userId, Address pickup, Address destination, Money estimatedFare)
    {
        Id = id;
        UserId = userId;
        Pickup = pickup;
        Destination = destination;
        EstimatedFare = estimatedFare;
        Status = OrderStatus.Created;
        CreatedAt = DateTime.UtcNow;
    }

    // FACTORY (Result<T>)
    public static Result<RideOrder> Create(Guid id, Guid userId, Address pickup, Address destination, Money estimatedFare)
    {
        if (id == Guid.Empty)
            return Result<RideOrder>.Failure("RideOrder ID cannot be empty.");

        if (userId == Guid.Empty)
            return Result<RideOrder>.Failure("User ID cannot be empty.");

        if (pickup is null)
            return Result<RideOrder>.Failure("Pickup address is required.");

        if (destination is null)
            return Result<RideOrder>.Failure("Destination address is required.");

        if (estimatedFare is null)
            return Result<RideOrder>.Failure("Estimated fare is required.");

        var order = new RideOrder(id, userId, pickup, destination, estimatedFare);

        order.AddDomainEvent(new RideCreatedEvent(id, userId, pickup, destination));

        return Result<RideOrder>.Success(order);
    }

    // Domain Event Helpers
    private void AddDomainEvent(IDomainEvent @event) => _domainEvents.Add(@event);
    public void ClearDomainEvents() => _domainEvents.Clear();


    // BEHAVIOR / STATE TRANSITIONS
    public void Accept(Guid driverId)
    {
        if (driverId == Guid.Empty)
            throw new ArgumentException(nameof(driverId));

        if (Status != OrderStatus.Created && Status != OrderStatus.Rejected)
            throw new BusinessRuleViolationException($"Cannot accept ride in status {Status}");

        DriverId = driverId;
        Status = OrderStatus.Accepted;
        AcceptedAt = DateTime.UtcNow;

        AddDomainEvent(new RideAcceptedEvent(Id, driverId));
    }

    public void Reject(Guid driverId)
    {
        if (driverId == Guid.Empty)
            throw new ArgumentException(nameof(driverId));

        if (Status != OrderStatus.Created)
            throw new BusinessRuleViolationException("Only created rides can be rejected.");

        DriverId = driverId;
        Status = OrderStatus.Rejected;
    }

    public void StartArriving()
    {
        if (Status != OrderStatus.Accepted)
            throw new BusinessRuleViolationException("Ride must be accepted before the driver can start arriving.");

        Status = OrderStatus.DriverArriving;
    }

    public void StartRide()
    {
        if (Status != OrderStatus.DriverArriving && Status != OrderStatus.Accepted)
            throw new BusinessRuleViolationException("Ride must be accepted or arriving before starting.");

        Status = OrderStatus.InProgress;
    }

    public void Complete(Money finalFare)
    {
        if (Status != OrderStatus.InProgress)
            throw new BusinessRuleViolationException("Ride must be in progress to complete.");

        FinalFare = finalFare ?? throw new ArgumentNullException(nameof(finalFare));
        Status = OrderStatus.Completed;
        CompletedAt = DateTime.UtcNow;

        AddDomainEvent(new RideCompletedEvent(Id, DriverId ?? Guid.Empty, UserId));
    }

    public void CancelByUser(string reason)
    {
        if (Status is OrderStatus.Completed or OrderStatus.Cancelled)
            throw new BusinessRuleViolationException("Cannot cancel a completed or already cancelled ride.");

        if (Status == OrderStatus.InProgress)
            throw new BusinessRuleViolationException("Cannot cancel a ride in progress.");

        Status = OrderStatus.Cancelled;
        CancellationReason = reason;
    }

    public void UpdateEstimatedFare(Money newEstimate)
    {
        if (newEstimate is null)
            throw new ArgumentNullException(nameof(newEstimate));

        if (Status != OrderStatus.Created)
            throw new BusinessRuleViolationException("Estimated fare can only be updated before the ride is accepted.");

        EstimatedFare = newEstimate;
    }
}
