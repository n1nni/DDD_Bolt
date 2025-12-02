using Bolt.Domain.Abstractions;
using Bolt.Domain.Shared;
using Bolt.Domain.ValueObjects;

namespace Bolt.Domain.Entities;

public sealed class Driver : IAggregateRoot
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public string FullName { get; private set; }
    public string VehicleDescription { get; private set; }
    public bool IsAvailable { get; private set; }
    public Rating? Rating { get; private set; }

    private readonly List<Guid> _completedRideIds = new();
    public IReadOnlyCollection<Guid> CompletedRideIds => _completedRideIds.AsReadOnly();
    
    private Driver() { }

    private Driver(Guid id, Guid userId, string fullName, string vehicleDescription)
    {
        Id = id;
        UserId = userId;
        FullName = fullName;
        VehicleDescription = vehicleDescription;
        IsAvailable = true;
    }

    // Factory Method
    public static Result<Driver> Create(
        Guid id,
        Guid userId,
        string fullName,
        string vehicleDescription)
    {
        if (id == Guid.Empty)
            return Result<Driver>.Failure("Driver id cannot be empty.");

        if (userId == Guid.Empty)
            return Result<Driver>.Failure("User id cannot be empty.");

        if (string.IsNullOrWhiteSpace(fullName))
            return Result<Driver>.Failure("Driver full name cannot be empty.");

        var driver = new Driver(
            id,
            userId,
            fullName.Trim(),
            vehicleDescription?.Trim() ?? string.Empty
        );

        return Result<Driver>.Success(driver);
    }

    // Behaviors (Methods)
    public void SetAvailability(bool available)
    {
        IsAvailable = available;
    }

    public Result<bool> AddCompletedRide(Guid rideOrderId)
    {
        if (rideOrderId == Guid.Empty)
            return Result<bool>.Failure("Ride order ID cannot be empty.");

        if (_completedRideIds.Contains(rideOrderId))
            return Result<bool>.Failure("Ride already exists in the completed list.");

        _completedRideIds.Add(rideOrderId);
        return Result<bool>.Success(true);
    }

    public Result<bool> UpdateRating(Rating rating)
    {
        if (rating is null)
            return Result<bool>.Failure("Rating cannot be null.");

        Rating = rating;
        return Result<bool>.Success(true);
    }
}
