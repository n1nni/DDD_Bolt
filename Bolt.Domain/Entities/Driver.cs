using Bolt.Domain.Abstractions;
using Bolt.Domain.Enums;
using Bolt.Domain.Events;
using Bolt.Domain.Shared;
using Bolt.Domain.ValueObjects;
using System.Data;

namespace Bolt.Domain.Entities;

public sealed class Driver : User
{
    public string LicenseNumber { get; private set; }
    public string VehicleModel { get; private set; }
    public string VehiclePlateNumber { get; private set; }
    public bool IsAvailable { get; private set; }
    public Rating? Rating { get; private set; }

    private readonly List<Guid> _completedRideIds = new();
    public IReadOnlyCollection<Guid> CompletedRideIds => _completedRideIds.AsReadOnly();

    private Driver() { } // EF Core

    internal Driver(Guid id, string fullName, string email, string phoneNumber)
        : base(id, fullName, email, phoneNumber, UserRole.Driver)
    {
        IsAvailable = true;
    }

    public static Result<Driver> CreateDriver(
        Guid id,
        string fullName,
        string email,
        string phoneNumber,
        string licenseNumber,
        string vehicleModel,
        string vehiclePlateNumber)
    {
        if (id == Guid.Empty)
            return Result<Driver>.Failure("Driver ID cannot be empty.");

        if (string.IsNullOrWhiteSpace(fullName))
            return Result<Driver>.Failure("Full name is required.");

        if (string.IsNullOrWhiteSpace(email))
            return Result<Driver>.Failure("Email is required.");

        if (string.IsNullOrWhiteSpace(phoneNumber))
            return Result<Driver>.Failure("Phone number is required.");

        if (string.IsNullOrWhiteSpace(licenseNumber))
            return Result<Driver>.Failure("License number is required.");

        if (string.IsNullOrWhiteSpace(vehicleModel))
            return Result<Driver>.Failure("Vehicle model is required.");

        if (string.IsNullOrWhiteSpace(vehiclePlateNumber))
            return Result<Driver>.Failure("Vehicle plate number is required.");

        var driver = new Driver(id, fullName.Trim(), email.Trim().ToLowerInvariant(), phoneNumber.Trim())
        {
            LicenseNumber = licenseNumber.Trim(),
            VehicleModel = vehicleModel.Trim(),
            VehiclePlateNumber = vehiclePlateNumber.Trim().ToUpperInvariant()
        };

        Console.WriteLine($"[LOG] Driver created: {driver.Id} - {driver.FullName}");
        return Result<Driver>.Success(driver);
    }

    public void SetAvailability(bool available)
    {
        IsAvailable = available;
        // Add domain event
        AddDomainEvent(new DriverAvailabilityChangedEvent(Id, available));
        Console.WriteLine($"[LOG] Driver availability changed: {Id} - Available: {available}");
    }

    public Result<bool> AddCompletedRide(Guid rideId)
    {
        if (rideId == Guid.Empty)
            return Result<bool>.Failure("Ride ID cannot be empty.");

        if (_completedRideIds.Contains(rideId))
            return Result<bool>.Failure("Ride already exists in completed list.");

        _completedRideIds.Add(rideId);
        Console.WriteLine($"[LOG] Completed ride added to driver: Driver {Id}, Ride {rideId}");

        return Result<bool>.Success(true);
    }

    public void UpdateRating(Rating rating)
    {
        Rating = rating ?? throw new ArgumentNullException(nameof(rating));
        Console.WriteLine($"[LOG] Driver rating updated: {Id} - New rating: {rating}");
    }
}