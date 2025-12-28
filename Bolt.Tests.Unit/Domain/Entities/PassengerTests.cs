using Bolt.Domain.Entities;
using Bolt.Domain.Enums;
using FluentAssertions;

namespace Bolt.Tests.Unit.Domain.Entities;

public class PassengerTests
{
    private readonly Guid _passengerId = Guid.NewGuid();
    private const string FullName = "Passenger Name";
    private const string Email = "passenger@example.com";
    private const string PhoneNumber = "+995123456789";

    [Fact]
    public void Create_WithValidParameters_ReturnsSuccessResult()
    {
        // Act
        var result = User.Create(_passengerId, FullName, Email, PhoneNumber, UserRole.Passenger);

        // Assert
        result.IsSuccess.Should().BeTrue();
        var passenger = result.Value as Passenger;
        passenger.Should().NotBeNull();
        passenger!.Id.Should().Be(_passengerId);
        passenger.Role.Should().Be(UserRole.Passenger);
        passenger.RideHistoryIds.Should().BeEmpty();
    }

    [Fact]
    public void AddRideToHistory_WithValidRideId_AddsToHistory()
    {
        // Arrange
        var passenger = User.Create(_passengerId, FullName, Email, PhoneNumber, UserRole.Passenger).Value as Passenger;
        var rideId = Guid.NewGuid();

        // Act
        passenger!.AddRideToHistory(rideId);

        // Assert
        passenger.RideHistoryIds.Should().Contain(rideId);
    }

    [Fact]
    public void AddRideToHistory_WithEmptyRideId_ThrowsException()
    {
        // Arrange
        var passenger = User.Create(_passengerId, FullName, Email, PhoneNumber, UserRole.Passenger).Value as Passenger;

        // Act & Assert
        Assert.Throws<ArgumentException>(() => passenger!.AddRideToHistory(Guid.Empty));
    }

    [Fact]
    public void AddRideToHistory_WithDuplicateRideId_DoesNotAddDuplicate()
    {
        // Arrange
        var passenger = User.Create(_passengerId, FullName, Email, PhoneNumber, UserRole.Passenger).Value as Passenger;
        var rideId = Guid.NewGuid();
        passenger!.AddRideToHistory(rideId); // First time

        // Act
        passenger.AddRideToHistory(rideId); // Second time

        // Assert
        passenger.RideHistoryIds.Should().ContainSingle();
    }
}
