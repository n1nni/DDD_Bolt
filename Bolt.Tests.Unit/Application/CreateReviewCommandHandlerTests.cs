using Bolt.Application.Abstractions;
using Bolt.Application.Features.Reviews.Commands.CreateReview;
using Bolt.Domain.Entities;
using Bolt.Domain.Enums;
using Bolt.Domain.Repositories;
using Bolt.Domain.ValueObjects;
using FluentAssertions;
using Moq;

namespace Bolt.Tests.Unit.Application;

public class CreateReviewCommandHandlerTests
{
    private readonly Mock<IReviewRepository> _reviewRepositoryMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IRideOrderRepository> _rideRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly CreateReviewCommandHandler _handler;

    private readonly Guid _rideId = Guid.NewGuid();
    private readonly Guid _driverId = Guid.NewGuid();
    private readonly Guid _passengerId = Guid.NewGuid();

    public CreateReviewCommandHandlerTests()
    {
        _reviewRepositoryMock = new Mock<IReviewRepository>();
        _userRepositoryMock = new Mock<IUserRepository>();
        _rideRepositoryMock = new Mock<IRideOrderRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        _handler = new CreateReviewCommandHandler(
            _reviewRepositoryMock.Object,
            _userRepositoryMock.Object,
            _rideRepositoryMock.Object,
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
        ride.Complete(new Money(18.75m, "GEL"));

        var command = new CreateReviewCommand(
            _rideId,
            _passengerId,
            _driverId,
            4.5,
            "Great driver!");

        _rideRepositoryMock
            .Setup(r => r.GetByIdAsync(_rideId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ride);

        _userRepositoryMock
            .Setup(r => r.GetDriverByIdAsync(_driverId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(driver);

        _userRepositoryMock
            .Setup(r => r.GetPassengerByIdAsync(_passengerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(passenger);

        _reviewRepositoryMock
            .Setup(r => r.GetByDriverAndPassengerAsync(_driverId, _passengerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Review?)null);

        _reviewRepositoryMock
            .Setup(r => r.GetByRideIdAsync(_rideId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Review?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        driver.Rating.Should().NotBeNull();

        _reviewRepositoryMock.Verify(
            r => r.AddAsync(It.IsAny<Review>(), It.IsAny<CancellationToken>()),
            Times.Once);

        _userRepositoryMock.Verify(
            r => r.Update(driver),
            Times.Once);

        _unitOfWorkMock.Verify(
            u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WithNonCompletedRide_ReturnsFailure()
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

        var command = new CreateReviewCommand(
            _rideId,
            _passengerId,
            _driverId,
            4.5,
            "Great driver!");

        _rideRepositoryMock
            .Setup(r => r.GetByIdAsync(_rideId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ride);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Can only review completed rides.");
    }

    [Fact]
    public async Task Handle_WithDuplicateReview_ReturnsFailure()
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
        ride.Complete(new Money(18.75m, "GEL"));

        var existingReview = Review.Create(
            Guid.NewGuid(),
            Guid.NewGuid(), // Different ride
            driver,
            passenger!,
            new Bolt.Domain.ValueObjects.Rating(4.0),
            "Previous review").Value!;

        var command = new CreateReviewCommand(
            _rideId,
            _passengerId,
            _driverId,
            4.5,
            "Great driver!");

        _rideRepositoryMock
            .Setup(r => r.GetByIdAsync(_rideId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ride);

        _userRepositoryMock
            .Setup(r => r.GetDriverByIdAsync(_driverId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(driver);

        _userRepositoryMock
            .Setup(r => r.GetPassengerByIdAsync(_passengerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(passenger);

        _reviewRepositoryMock
            .Setup(r => r.GetByDriverAndPassengerAsync(_driverId, _passengerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingReview);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("You have already reviewed this driver.");
    }
}