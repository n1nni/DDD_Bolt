using Bolt.Application.Abstractions;
using Bolt.Application.Features.Rides.Commands.CompleteRide;
using Bolt.Domain.Entities;
using Bolt.Domain.Enums;
using Bolt.Domain.Repositories;
using Bolt.Domain.ValueObjects;
using FluentAssertions;
using Moq;

namespace Bolt.Tests.Unit.Application;

public class CompleteRideCommandHandlerTests
{
    private readonly Mock<IRideOrderRepository> _rideRepositoryMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly CompleteRideCommandHandler _handler;

    private readonly Guid _rideId = Guid.NewGuid();
    private readonly Guid _driverId = Guid.NewGuid();
    private readonly Guid _passengerId = Guid.NewGuid();

    public CompleteRideCommandHandlerTests()
    {
        _rideRepositoryMock = new Mock<IRideOrderRepository>();
        _userRepositoryMock = new Mock<IUserRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        _handler = new CompleteRideCommandHandler(
            _rideRepositoryMock.Object,
            _userRepositoryMock.Object,
            _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidCommand_ReturnsSuccess()
    {
        // Arrange
        var passenger = User.Create(
            _passengerId,
            "John Passenger",
            "john@example.com",
            "+995123456789",
            UserRole.Passenger).Value as Passenger;

        var driver = Driver.CreateDriver(
            _driverId,
            "John Driver",
            "driver@example.com",
            "+995987654321",
            "DL123456",
            "Toyota Prius",
            "TB-123-AA").Value!;

        var ride = RideOrder.Create(
            _rideId,
            passenger!,
            new Address("Pickup St", "City", new Location(41.6938, 44.8015)),
            new Address("Dest St", "City", new Location(41.7167, 44.7833)),
            new Money(15.50m)).Value!;

        ride.Accept(driver);
        ride.Start();

        var command = new CompleteRideCommand(_rideId, 18.75m, "GEL");

        _rideRepositoryMock
            .Setup(r => r.GetByIdAsync(_rideId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ride);

        _userRepositoryMock
            .Setup(r => r.GetDriverByIdAsync(_driverId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(driver);

        _userRepositoryMock
            .Setup(r => r.GetPassengerByIdAsync(_passengerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(passenger);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        ride.Status.Should().Be(RideStatus.Completed);
        ride.FinalFare.Should().NotBeNull();
        ride.FinalFare!.Amount.Should().Be(18.75m);
        driver.IsAvailable.Should().BeTrue();
        driver.CompletedRideIds.Should().Contain(_rideId);

        _rideRepositoryMock.Verify(
            r => r.Update(ride),
            Times.Once);

        _userRepositoryMock.Verify(
            r => r.Update(driver),
            Times.Once);

        _userRepositoryMock.Verify(
            r => r.Update(passenger!),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WithNonExistentRide_ReturnsFailure()
    {
        // Arrange
        var command = new CompleteRideCommand(_rideId, 18.75m, "GEL");

        _rideRepositoryMock
            .Setup(r => r.GetByIdAsync(_rideId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((RideOrder?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Ride not found.");
    }
}