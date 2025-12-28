using Bolt.Application.Abstractions;
using Bolt.Application.Features.Rides.Commands.AcceptRide;
using Bolt.Domain.Entities;
using Bolt.Domain.Enums;
using Bolt.Domain.Repositories;
using Bolt.Domain.ValueObjects;
using FluentAssertions;
using Moq;

namespace Bolt.Tests.Unit.Application;

public class AcceptRideCommandHandlerTests
{
    private readonly Mock<IRideOrderRepository> _rideRepositoryMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly AcceptRideCommandHandler _handler;

    private readonly Guid _rideId = Guid.NewGuid();
    private readonly Guid _driverId = Guid.NewGuid();
    private readonly Guid _passengerId = Guid.NewGuid();

    public AcceptRideCommandHandlerTests()
    {
        _rideRepositoryMock = new Mock<IRideOrderRepository>();
        _userRepositoryMock = new Mock<IUserRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        _handler = new AcceptRideCommandHandler(
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

        var ride = RideOrder.Create(
            _rideId,
            passenger!,
            new Address("Pickup St", "City", new Location(41.6938, 44.8015)),
            new Address("Dest St", "City", new Location(41.7167, 44.7833)),
            new Money(15.50m)).Value!;

        var driver = Driver.CreateDriver(
            _driverId,
            "John Driver",
            "driver@example.com",
            "+995987654321",
            "DL123456",
            "Toyota Prius",
            "TB-123-AA").Value!;

        var command = new AcceptRideCommand(_rideId, _driverId);

        _rideRepositoryMock
            .Setup(r => r.GetByIdAsync(_rideId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ride);

        _userRepositoryMock
            .Setup(r => r.GetDriverByIdAsync(_driverId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(driver);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        ride.DriverId.Should().Be(_driverId);
        ride.Status.Should().Be(RideStatus.Accepted);
        driver.IsAvailable.Should().BeFalse();

        _rideRepositoryMock.Verify(
            r => r.Update(ride),
            Times.Once);

        _userRepositoryMock.Verify(
            r => r.Update(driver),
            Times.Once);

        _unitOfWorkMock.Verify(
            u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WithNonExistentRide_ReturnsFailure()
    {
        // Arrange
        var command = new AcceptRideCommand(_rideId, _driverId);

        _rideRepositoryMock
            .Setup(r => r.GetByIdAsync(_rideId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((RideOrder?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Ride not found or has been deleted.");

        _rideRepositoryMock.Verify(
            r => r.Update(It.IsAny<RideOrder>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_WithNonExistentDriver_ReturnsFailure()
    {
        // Arrange
        var passenger = User.Create(
            _passengerId,
            "John Passenger",
            "john@example.com",
            "+995123456789",
            UserRole.Passenger).Value as Passenger;

        var ride = RideOrder.Create(
            _rideId,
            passenger!,
            new Address("Pickup St", "City", new Location(41.6938, 44.8015)),
            new Address("Dest St", "City", new Location(41.7167, 44.7833)),
            new Money(15.50m)).Value!;

        var command = new AcceptRideCommand(_rideId, _driverId);

        _rideRepositoryMock
            .Setup(r => r.GetByIdAsync(_rideId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ride);

        _userRepositoryMock
            .Setup(r => r.GetDriverByIdAsync(_driverId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Driver?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Driver not found or has been deleted.");

        _rideRepositoryMock.Verify(
            r => r.Update(It.IsAny<RideOrder>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_WithUnavailableDriver_ReturnsFailure()
    {
        // Arrange
        var passenger = User.Create(
            _passengerId,
            "John Passenger",
            "john@example.com",
            "+995123456789",
            UserRole.Passenger).Value as Passenger;

        var ride = RideOrder.Create(
            _rideId,
            passenger!,
            new Address("Pickup St", "City", new Location(41.6938, 44.8015)),
            new Address("Dest St", "City", new Location(41.7167, 44.7833)),
            new Money(15.50m)).Value!;

        var driver = Driver.CreateDriver(
            _driverId,
            "John Driver",
            "driver@example.com",
            "+995987654321",
            "DL123456",
            "Toyota Prius",
            "TB-123-AA").Value!;

        driver.SetAvailability(false); // Make driver unavailable

        var command = new AcceptRideCommand(_rideId, _driverId);

        _rideRepositoryMock
            .Setup(r => r.GetByIdAsync(_rideId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ride);

        _userRepositoryMock
            .Setup(r => r.GetDriverByIdAsync(_driverId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(driver);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Driver is not available.");

        _rideRepositoryMock.Verify(
            r => r.Update(It.IsAny<RideOrder>()),
            Times.Never);
    }
}
