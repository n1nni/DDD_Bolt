using Bolt.Domain.Entities;
using Bolt.Domain.Enums;
using Bolt.Domain.Events;
using Bolt.Domain.ValueObjects;
using FluentAssertions;

namespace Bolt.Tests.Unit.Domain.Entities;

public class DriverTests
{
    private readonly Guid _driverId = Guid.NewGuid();
    private const string FullName = "Driver Name";
    private const string Email = "driver@example.com";
    private const string PhoneNumber = "+995123456789";
    private const string LicenseNumber = "DL123456";
    private const string VehicleModel = "Toyota Prius";
    private const string VehiclePlateNumber = "TB-123-AA";

    [Fact]
    public void CreateDriver_WithValidParameters_ReturnsSuccessResult()
    {
        // Act
        var result = Driver.CreateDriver(
            _driverId,
            FullName,
            Email,
            PhoneNumber,
            LicenseNumber,
            VehicleModel,
            VehiclePlateNumber);

        // Assert
        result.IsSuccess.Should().BeTrue();
        var driver = result.Value;
        driver.Should().NotBeNull();
        driver!.Id.Should().Be(_driverId);
        driver.LicenseNumber.Should().Be(LicenseNumber);
        driver.VehicleModel.Should().Be(VehicleModel);
        driver.VehiclePlateNumber.Should().Be(VehiclePlateNumber.ToUpperInvariant());
        driver.IsAvailable.Should().BeTrue();
        driver.Role.Should().Be(UserRole.Driver);


    }

    [Theory]
    [InlineData("dl-123-456", "DL-123-456")] // Should uppercase
    [InlineData("tb-123-aa", "TB-123-AA")] // Should uppercase and trim
    public void CreateDriver_WithVehiclePlate_TransformsToUppercase(string input, string expected)
    {
        // Act
        var result = Driver.CreateDriver(
            _driverId,
            FullName,
            Email,
            PhoneNumber,
            LicenseNumber,
            VehicleModel,
            input);

        // Assert
        result.Value!.VehiclePlateNumber.Should().Be(expected);
    }

    [Fact]
    public void SetAvailability_UpdatesAvailabilityAndPublishesEvent()
    {
        // Arrange
        var driver = Driver.CreateDriver(
            _driverId,
            FullName,
            Email,
            PhoneNumber,
            LicenseNumber,
            VehicleModel,
            VehiclePlateNumber).Value!;

        driver.ClearDomainEvents(); // Clear initial created event

        // Act
        driver.SetAvailability(false);

        // Assert
        driver.IsAvailable.Should().BeFalse();
        driver.DomainEvents.Should().ContainSingle(e => e is DriverAvailabilityChangedEvent);
        var @event = driver.DomainEvents.First() as DriverAvailabilityChangedEvent;
        @event!.UserId.Should().Be(_driverId);
        @event.IsAvailable.Should().BeFalse();
    }

    [Fact]
    public void AddCompletedRide_WithValidRideId_ReturnsSuccess()
    {
        // Arrange
        var driver = Driver.CreateDriver(
            _driverId,
            FullName,
            Email,
            PhoneNumber,
            LicenseNumber,
            VehicleModel,
            VehiclePlateNumber).Value!;
        var rideId = Guid.NewGuid();

        // Act
        var result = driver.AddCompletedRide(rideId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        driver.CompletedRideIds.Should().Contain(rideId);
    }

    [Fact]
    public void AddCompletedRide_WithEmptyRideId_ReturnsFailure()
    {
        // Arrange
        var driver = Driver.CreateDriver(
            _driverId,
            FullName,
            Email,
            PhoneNumber,
            LicenseNumber,
            VehicleModel,
            VehiclePlateNumber).Value!;

        // Act
        var result = driver.AddCompletedRide(Guid.Empty);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Ride ID cannot be empty.");
    }

    [Fact]
    public void AddCompletedRide_WithDuplicateRideId_ReturnsFailure()
    {
        // Arrange
        var driver = Driver.CreateDriver(
            _driverId,
            FullName,
            Email,
            PhoneNumber,
            LicenseNumber,
            VehicleModel,
            VehiclePlateNumber).Value!;
        var rideId = Guid.NewGuid();
        driver.AddCompletedRide(rideId); // First time

        // Act
        var result = driver.AddCompletedRide(rideId); // Second time

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Ride already exists in completed list.");
        driver.CompletedRideIds.Should().ContainSingle();
    }

    [Fact]
    public void UpdateRating_SetsNewRating()
    {
        // Arrange
        var driver = Driver.CreateDriver(
            _driverId,
            FullName,
            Email,
            PhoneNumber,
            LicenseNumber,
            VehicleModel,
            VehiclePlateNumber).Value!;
        var rating = new Rating(4.5);

        // Act
        driver.UpdateRating(rating);

        // Assert
        driver.Rating.Should().Be(rating);
    }

    [Fact]
    public void UpdateRating_WithNullRating_ThrowsException()
    {
        // Arrange
        var driver = Driver.CreateDriver(
            _driverId,
            FullName,
            Email,
            PhoneNumber,
            LicenseNumber,
            VehicleModel,
            VehiclePlateNumber).Value!;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => driver.UpdateRating(null!));
    }
}
