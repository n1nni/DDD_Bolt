using Bolt.Domain.Entities;
using Bolt.Domain.Enums;
using Bolt.Domain.Events;
using Bolt.Domain.ValueObjects;
using FluentAssertions;

namespace Bolt.Tests.Unit.Domain.Entities;

public class RideOrderTests
{
    private readonly Guid _rideId = Guid.NewGuid();
    private readonly Guid _passengerId = Guid.NewGuid();
    private readonly Passenger _passenger;
    private readonly Driver _driver;
    private readonly Address _pickupAddress;
    private readonly Address _destinationAddress;
    private readonly Money _estimatedFare;

    public RideOrderTests()
    {
        _passenger = User.Create(
            _passengerId,
            "Passenger Name",
            "passenger@example.com",
            "+995123456789",
            UserRole.Passenger).Value as Passenger ?? throw new Exception("Failed to create passenger");

        var driverId = Guid.NewGuid();
        _driver = Driver.CreateDriver(
            driverId,
            "Driver Name",
            "driver@example.com",
            "+995987654321",
            "DL123456",
            "Toyota Prius",
            "TB-123-AA").Value!;

        _pickupAddress = new Address(
            "Rustaveli Ave 14",
            "Tbilisi",
            new Location(41.6938, 44.8015),
            "0108");

        _destinationAddress = new Address(
            "University St 13",
            "Tbilisi",
            new Location(41.7167, 44.7833),
            "0179");

        _estimatedFare = new Money(15.50m, "GEL");
    }

    [Fact]
    public void Create_WithValidParameters_ReturnsSuccessResult()
    {
        // Act
        var result = RideOrder.Create(_rideId, _passenger, _pickupAddress, _destinationAddress, _estimatedFare);

        // Assert
        result.IsSuccess.Should().BeTrue();
        var ride = result.Value;
        ride.Should().NotBeNull();
        ride!.Id.Should().Be(_rideId);
        ride.PassengerId.Should().Be(_passengerId);
        ride.PickupAddress.Should().Be(_pickupAddress);
        ride.DestinationAddress.Should().Be(_destinationAddress);
        ride.EstimatedFare.Should().Be(_estimatedFare);
        ride.Status.Should().Be(RideStatus.Created);
        ride.DriverId.Should().BeNull();
        ride.FinalFare.Should().BeNull();
        ride.DomainEvents.Should().ContainSingle(e => e is RideCreatedEvent);
    }

    [Fact]
    public void Accept_WithAvailableDriver_AcceptsRide()
    {
        // Arrange
        var ride = RideOrder.Create(_rideId, _passenger, _pickupAddress, _destinationAddress, _estimatedFare).Value!;
        ride.ClearDomainEvents();

        // Act
        var result = ride.Accept(_driver);

        // Assert
        result.IsSuccess.Should().BeTrue();
        ride.DriverId.Should().Be(_driver.Id);
        ride.Status.Should().Be(RideStatus.Accepted);
        ride.AcceptedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        ride.DomainEvents.Should().ContainSingle(e => e is RideAcceptedEvent);
    }

    [Fact]
    public void Accept_WithNullDriver_ReturnsFailure()
    {
        // Arrange
        var ride = RideOrder.Create(_rideId, _passenger, _pickupAddress, _destinationAddress, _estimatedFare).Value!;

        // Act
        var result = ride.Accept(null!);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Driver cannot be null.");
    }

    [Fact]
    public void Accept_WhenRideNotInCreatedStatus_ReturnsFailure()
    {
        // Arrange
        var ride = RideOrder.Create(_rideId, _passenger, _pickupAddress, _destinationAddress, _estimatedFare).Value!;
        ride.Accept(_driver);
        ride.Start();

        // Try to accept again
        var anotherDriver = Driver.CreateDriver(
            Guid.NewGuid(),
            "Another Driver",
            "another@example.com",
            "+995555555555",
            "DL999999",
            "Tesla Model 3",
            "TB-999-ZZ").Value!;

        // Act
        var result = ride.Accept(anotherDriver);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be($"Cannot accept ride in status {RideStatus.InProgress}.");
    }

    [Fact]
    public void Start_WhenRideAccepted_StartsRide()
    {
        // Arrange
        var ride = RideOrder.Create(_rideId, _passenger, _pickupAddress, _destinationAddress, _estimatedFare).Value!;
        ride.Accept(_driver);
        ride.ClearDomainEvents();

        // Act
        var result = ride.Start();

        // Assert
        result.IsSuccess.Should().BeTrue();
        ride.Status.Should().Be(RideStatus.InProgress);
        ride.StartedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        ride.DomainEvents.Should().ContainSingle(e => e is RideStartedEvent);
    }


    [Fact]
    public void Complete_WithValidParameters_CompletesRide()
    {
        // Arrange
        var ride = RideOrder.Create(_rideId, _passenger, _pickupAddress, _destinationAddress, _estimatedFare).Value!;
        ride.Accept(_driver);
        ride.Start();
        ride.ClearDomainEvents();
        var finalFare = new Money(18.75m, "GEL");

        // Act
        var result = ride.Complete(finalFare);

        // Assert
        result.IsSuccess.Should().BeTrue();
        ride.Status.Should().Be(RideStatus.Completed);
        ride.FinalFare.Should().Be(finalFare);
        ride.CompletedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        ride.DomainEvents.Should().ContainSingle(e => e is RideCompletedEvent);
    }

    [Fact]
    public void Complete_WithoutDriver_ReturnsFailure()
    {
        // Arrange
        var ride = RideOrder.Create(_rideId, _passenger, _pickupAddress, _destinationAddress, _estimatedFare).Value!;
        var finalFare = new Money(18.75m, "GEL");

        // Act
        var result = ride.Complete(finalFare);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("No driver assigned to this ride.");
    }

    [Fact]
    public void Complete_WithNullFinalFare_ReturnsFailure()
    {
        // Arrange
        var ride = RideOrder.Create(_rideId, _passenger, _pickupAddress, _destinationAddress, _estimatedFare).Value!;
        ride.Accept(_driver);
        ride.Start();

        // Act
        var result = ride.Complete(null!);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Final fare is required.");
    }

    [Fact]
    public void Cancel_WithValidParameters_CancelsRide()
    {
        // Arrange
        var ride = RideOrder.Create(_rideId, _passenger, _pickupAddress, _destinationAddress, _estimatedFare).Value!;
        ride.ClearDomainEvents();
        var cancelledBy = _passengerId;
        var reason = "Passenger changed mind";

        // Act
        var result = ride.Cancel(cancelledBy, reason);

        // Assert
        result.IsSuccess.Should().BeTrue();
        ride.Status.Should().Be(RideStatus.Cancelled);
        ride.CancelledBy.Should().Be(cancelledBy);
        ride.CancellationReason.Should().Be(reason);
        ride.CancelledAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        ride.DomainEvents.Should().ContainSingle(e => e.EventName == "RideCancelled");
    }

    [Fact]
    public void Cancel_WhenRideCompleted_ReturnsFailure()
    {
        // Arrange
        var ride = RideOrder.Create(_rideId, _passenger, _pickupAddress, _destinationAddress, _estimatedFare).Value!;
        ride.Accept(_driver);
        ride.Start();
        ride.Complete(new Money(18.75m, "GEL"));

        // Act
        var result = ride.Cancel(_passengerId, "Too late");

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Cannot cancel a completed ride.");
        ride.Status.Should().Be(RideStatus.Completed);
    }

    [Fact]
    public void Cancel_WhenRideAlreadyCancelled_ReturnsFailure()
    {
        // Arrange
        var ride = RideOrder.Create(_rideId, _passenger, _pickupAddress, _destinationAddress, _estimatedFare).Value!;
        ride.Cancel(_passengerId, "First cancellation");

        // Act
        var result = ride.Cancel(_passengerId, "Second attempt");

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Ride is already cancelled.");
        ride.Status.Should().Be(RideStatus.Cancelled);
    }

    [Fact]
    public void Cancel_WhenRideInProgress_ReturnsFailure()
    {
        // Arrange
        var ride = RideOrder.Create(_rideId, _passenger, _pickupAddress, _destinationAddress, _estimatedFare).Value!;
        ride.Accept(_driver);
        ride.Start();

        // Act
        var result = ride.Cancel(_passengerId, "Want to stop");

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Cannot cancel a ride in progress.");
        ride.Status.Should().Be(RideStatus.InProgress);
    }

    [Fact]
    public void Cancel_WithEmptyReason_WorksWithNull()
    {
        // Arrange
        var ride = RideOrder.Create(_rideId, _passenger, _pickupAddress, _destinationAddress, _estimatedFare).Value!;

        // Act
        var result = ride.Cancel(_passengerId, null!);

        // Assert
        result.IsSuccess.Should().BeTrue();
        ride.CancellationReason.Should().BeNull();
    }

    [Fact]
    public void Cancel_TrimsReasonString()
    {
        // Arrange
        var ride = RideOrder.Create(_rideId, _passenger, _pickupAddress, _destinationAddress, _estimatedFare).Value!;
        const string reasonWithSpaces = "  Changed my mind  ";

        // Act
        var result = ride.Cancel(_passengerId, reasonWithSpaces);

        // Assert
        result.IsSuccess.Should().BeTrue();
        ride.CancellationReason.Should().Be("Changed my mind");
    }

    [Fact]
    public void Cancel_ByDriver_WorksWhenDriverAccepted()
    {
        // Arrange
        var ride = RideOrder.Create(_rideId, _passenger, _pickupAddress, _destinationAddress, _estimatedFare).Value!;
        ride.Accept(_driver);

        // Act
        var result = ride.Cancel(_driver.Id, "Driver unavailable");

        // Assert
        result.IsSuccess.Should().BeTrue();
        ride.Status.Should().Be(RideStatus.Cancelled);
        ride.CancelledBy.Should().Be(_driver.Id);
    }

    [Fact]
    public void Accept_WithUnavailableDriver_ReturnsFailure()
    {
        // Arrange
        var ride = RideOrder.Create(_rideId, _passenger, _pickupAddress, _destinationAddress, _estimatedFare).Value!;
        _driver.SetAvailability(false);

        // Act
        var result = ride.Accept(_driver);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Driver is not available.");
        ride.Status.Should().Be(RideStatus.Created);
        ride.DriverId.Should().BeNull();
    }

    [Fact]
    public void Start_WithoutDriver_ReturnsFailure()
    {
        // Arrange
        var ride = RideOrder.Create(_rideId, _passenger, _pickupAddress, _destinationAddress, _estimatedFare).Value!;

        // Act
        var result = ride.Start();

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("No driver assigned to this ride.");
        ride.Status.Should().Be(RideStatus.Created);
    }
    

    [Fact]
    public void StatusFlow_FromCreatedToCompleted_WorksCorrectly()
    {
        // Arrange
        var ride = RideOrder.Create(_rideId, _passenger, _pickupAddress, _destinationAddress, _estimatedFare).Value!;

        // Act & Assert - Step 1: Accept
        var acceptResult = ride.Accept(_driver);
        acceptResult.IsSuccess.Should().BeTrue();
        ride.Status.Should().Be(RideStatus.Accepted);
        ride.DriverId.Should().Be(_driver.Id);

        // Step 2: Start
        var startResult = ride.Start();
        startResult.IsSuccess.Should().BeTrue();
        ride.Status.Should().Be(RideStatus.InProgress);
        ride.StartedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));

        // Step 3: Complete
        var finalFare = new Money(18.75m, "GEL");
        var completeResult = ride.Complete(finalFare);
        completeResult.IsSuccess.Should().BeTrue();
        ride.Status.Should().Be(RideStatus.Completed);
        ride.FinalFare.Should().Be(finalFare);
        ride.CompletedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void StatusFlow_WithCancellation_WorksCorrectly()
    {
        // Arrange
        var ride = RideOrder.Create(_rideId, _passenger, _pickupAddress, _destinationAddress, _estimatedFare).Value!;

        // Act & Assert - Cancel before driver accepts
        var cancelResult = ride.Cancel(_passengerId, "Changed mind");
        cancelResult.IsSuccess.Should().BeTrue();
        ride.Status.Should().Be(RideStatus.Cancelled);
        ride.CancelledAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Properties_AreSetCorrectlyOnCreation()
    {
        // Act
        var ride = RideOrder.Create(_rideId, _passenger, _pickupAddress, _destinationAddress, _estimatedFare).Value!;

        // Assert
        ride.Id.Should().Be(_rideId);
        ride.PassengerId.Should().Be(_passengerId);
        ride.PickupAddress.Should().Be(_pickupAddress);
        ride.DestinationAddress.Should().Be(_destinationAddress);
        ride.EstimatedFare.Should().Be(_estimatedFare);
        ride.Status.Should().Be(RideStatus.Created);
        ride.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        ride.IsDeleted.Should().BeFalse();
        ride.DriverId.Should().BeNull();
        ride.FinalFare.Should().BeNull();
        ride.AcceptedAt.Should().BeNull();
        ride.StartedAt.Should().BeNull();
        ride.CompletedAt.Should().BeNull();
        ride.CancelledAt.Should().BeNull();
        ride.CancellationReason.Should().BeNull();
        ride.CancelledBy.Should().BeNull();
    }

    [Fact]
    public void Accept_SetsAcceptedAtTimestamp()
    {
        // Arrange
        var ride = RideOrder.Create(_rideId, _passenger, _pickupAddress, _destinationAddress, _estimatedFare).Value!;

        // Act
        ride.Accept(_driver);

        // Assert
        ride.AcceptedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Create_WithNullPassenger_ReturnsFailure()
    {
        // Act
        var result = RideOrder.Create(_rideId, null!, _pickupAddress, _destinationAddress, _estimatedFare);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Passenger is required.");
    }

    [Fact]
    public void Create_WithEmptyRideId_ReturnsFailure()
    {
        // Act
        var result = RideOrder.Create(Guid.Empty, _passenger, _pickupAddress, _destinationAddress, _estimatedFare);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Ride ID cannot be empty.");
    }
}
