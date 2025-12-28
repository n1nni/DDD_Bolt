using Bolt.Application.Abstractions;
using Bolt.Application.Features.Rides.Commands.CreateRide;
using Bolt.Domain.Entities;
using Bolt.Domain.Repositories;
using Bolt.Domain.Services;
using Bolt.Domain.ValueObjects;
using FluentAssertions;
using Moq;

namespace Bolt.Tests.Unit.Application;

public class CreateRideCommandHandlerTests
{
    private readonly Mock<IRideOrderRepository> _rideRepositoryMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IPricingService> _pricingServiceMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly CreateRideCommandHandler _handler;

    public CreateRideCommandHandlerTests()
    {
        _rideRepositoryMock = new Mock<IRideOrderRepository>();
        _userRepositoryMock = new Mock<IUserRepository>();
        _pricingServiceMock = new Mock<IPricingService>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        _handler = new CreateRideCommandHandler(
            _rideRepositoryMock.Object,
            _userRepositoryMock.Object,
            _pricingServiceMock.Object,
            _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidCommand_ReturnsSuccess()
    {
        // Arrange
        var passengerId = Guid.NewGuid();
        var passenger = User.Create(
            passengerId,
            "John Passenger",
            "john@example.com",
            "+995123456789",
            Bolt.Domain.Enums.UserRole.Passenger).Value as Passenger;

        var command = new CreateRideCommand(
            passengerId,
            "Rustaveli Ave 14",
            "Tbilisi",
            "0108",
            41.6938,
            44.8015,
            "University St 13",
            "Tbilisi",
            "0179",
            41.7167,
            44.7833);

        var estimatedFare = new Money(15.50m, "GEL");

        _userRepositoryMock
            .Setup(r => r.GetPassengerByIdAsync(passengerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(passenger);

        _pricingServiceMock
            .Setup(p => p.CalculateEstimatedFare(It.IsAny<Location>(), It.IsAny<Location>()))
            .Returns(estimatedFare);

        _rideRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<RideOrder>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBe(Guid.Empty);

        _userRepositoryMock.Verify(
            r => r.GetPassengerByIdAsync(passengerId, It.IsAny<CancellationToken>()),
            Times.Once);

        _pricingServiceMock.Verify(
            p => p.CalculateEstimatedFare(It.IsAny<Location>(), It.IsAny<Location>()),
            Times.Once);

        _rideRepositoryMock.Verify(
            r => r.AddAsync(It.IsAny<RideOrder>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WithNonExistentPassenger_ReturnsFailure()
    {
        // Arrange
        var command = new CreateRideCommand(
            Guid.NewGuid(),
            "Rustaveli Ave 14",
            "Tbilisi",
            "0108",
            41.6938,
            44.8015,
            "University St 13",
            "Tbilisi",
            "0179",
            41.7167,
            44.7833);

        _userRepositoryMock
            .Setup(r => r.GetPassengerByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Passenger?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Passenger not found.");

        _rideRepositoryMock.Verify(
            r => r.AddAsync(It.IsAny<RideOrder>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }
    [Fact]
    public async Task Handle_WithInvalidLocation_ThrowsException()
    {
        // Arrange
        var passengerId = Guid.NewGuid();
        var passenger = User.Create(
            passengerId,
            "John Passenger",
            "john@example.com",
            "+995123456789",
            Bolt.Domain.Enums.UserRole.Passenger).Value as Passenger;

        // Invalid latitude (> 90)
        var command = new CreateRideCommand(
            passengerId,
            "Rustaveli Ave 14",
            "Tbilisi",
            "0108",
            91.0,  // Invalid latitude
            44.8015,
            "University St 13",
            "Tbilisi",
            "0179",
            41.7167,
            44.7833);

        _userRepositoryMock
            .Setup(r => r.GetPassengerByIdAsync(passengerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(passenger);

        // Act & Assert
        // This will throw ArgumentOutOfRangeException when creating Location
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
            () => _handler.Handle(command, CancellationToken.None));
    }
}