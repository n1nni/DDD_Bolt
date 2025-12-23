using Bolt.Domain.Abstractions;
using Bolt.Domain.Enums;
using Bolt.Domain.Events;
using Bolt.Domain.Exceptions;
using Bolt.Domain.Shared;
using Bolt.Domain.ValueObjects;
using System.ComponentModel.DataAnnotations;

namespace Bolt.Domain.Entities;

public class RideOrder : IAggregateRoot
{
    public Guid Id { get; private set; }

    // Reference to the passenger who requested the ride
    public Guid PassengerId { get; private set; }

    // Reference to the driver who accepted the ride (nullable until accepted)
    public Guid? DriverId { get; private set; }

    public Address PickupAddress { get; private set; }
    public Address DestinationAddress { get; private set; }
    public Money EstimatedFare { get; private set; }
    public Money? FinalFare { get; private set; }
    public RideStatus Status { get; private set; }

    public DateTime CreatedAt { get; private set; }
    public DateTime? AcceptedAt { get; private set; }
    public DateTime? StartedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public DateTime? CancelledAt { get; private set; }

    public string? CancellationReason { get; private set; }
    public Guid? CancelledBy { get; private set; }

    public bool IsDeleted { get; private set; }

    private RideOrder() { }

    private RideOrder(
        Guid id,
        Guid passengerId,
        Address pickupAddress,
        Address destinationAddress,
        Money estimatedFare)
    {
        Id = id;
        PassengerId = passengerId;
        PickupAddress = pickupAddress;
        DestinationAddress = destinationAddress;
        EstimatedFare = estimatedFare;
        Status = RideStatus.Created;
        CreatedAt = DateTime.UtcNow;
        IsDeleted = false;
    }
  
    public static Result<RideOrder> Create(
        Guid id,
        Passenger passenger,
        Address pickupAddress,
        Address destinationAddress,
        Money estimatedFare)
    {
        if (id == Guid.Empty)
            return Result<RideOrder>.Failure("Ride ID cannot be empty.");

        if (passenger == null)
            return Result<RideOrder>.Failure("Passenger is required.");

        if (pickupAddress == null)
            return Result<RideOrder>.Failure("Pickup address is required.");

        if (destinationAddress == null)
            return Result<RideOrder>.Failure("Destination address is required.");

        if (estimatedFare == null)
            return Result<RideOrder>.Failure("Estimated fare is required.");

        var ride = new RideOrder(id, passenger.Id, pickupAddress, destinationAddress, estimatedFare);

        Console.WriteLine($"[LOG] Ride created: {id} by Passenger {passenger.Id}");

        return Result<RideOrder>.Success(ride);
    }

    public Result<bool> Accept(Driver driver)
    {
        if (driver == null)
            return Result<bool>.Failure("Driver cannot be null.");

        if (!driver.IsAvailable)
            return Result<bool>.Failure("Driver is not available.");

        if (Status != RideStatus.Created)
            return Result<bool>.Failure($"Cannot accept ride in status {Status}.");

        DriverId = driver.Id;
        Status = RideStatus.Accepted;
        AcceptedAt = DateTime.UtcNow;

        Console.WriteLine($"[LOG] Ride accepted: {Id} by Driver {driver.Id}");

        return Result<bool>.Success(true);
    }

    public Result<bool> Reject(Driver driver, string reason)
    {
        if (driver == null)
            return Result<bool>.Failure("Driver cannot be null.");

        if (Status != RideStatus.Created)
            return Result<bool>.Failure("Only created rides can be rejected.");

        Status = RideStatus.Rejected;
        CancellationReason = reason?.Trim();

        Console.WriteLine($"[LOG] Ride rejected: {Id} by Driver {driver.Id}. Reason: {reason}");

        return Result<bool>.Success(true);
    }

    public Result<bool> Start()
    {
        if (!DriverId.HasValue)
            return Result<bool>.Failure("No driver assigned to this ride.");

        if (Status != RideStatus.DriverArriving && Status != RideStatus.Accepted)
            return Result<bool>.Failure("Ride must be in 'Accepted' status to start.");

        Status = RideStatus.InProgress;
        StartedAt = DateTime.UtcNow;

        Console.WriteLine($"[LOG] Ride started: {Id}");

        return Result<bool>.Success(true);
    }

    public Result<bool> Complete(Money finalFare)
    {
        if (!DriverId.HasValue)
            return Result<bool>.Failure("No driver assigned to this ride.");

        if (Status != RideStatus.InProgress)
            return Result<bool>.Failure("Ride must be in progress to complete.");

        if (finalFare == null)
            return Result<bool>.Failure("Final fare is required.");

        FinalFare = finalFare;
        Status = RideStatus.Completed;
        CompletedAt = DateTime.UtcNow;

        Console.WriteLine($"[LOG] Ride completed: {Id}. Final fare: {finalFare}");

        return Result<bool>.Success(true);
    }

    public Result<bool> Cancel(Guid cancelledBy, string reason)
    {
        if (Status == RideStatus.Completed)
            return Result<bool>.Failure("Cannot cancel a completed ride.");

        if (Status == RideStatus.Cancelled)
            return Result<bool>.Failure("Ride is already cancelled.");

        if (Status == RideStatus.InProgress)
            return Result<bool>.Failure("Cannot cancel a ride in progress.");

        Status = RideStatus.Cancelled;
        CancellationReason = reason?.Trim();
        CancelledBy = cancelledBy;
        CancelledAt = DateTime.UtcNow;

        Console.WriteLine($"[LOG] Ride cancelled: {Id} by User {cancelledBy}. Reason: {reason}");

        return Result<bool>.Success(true);
    }
}
