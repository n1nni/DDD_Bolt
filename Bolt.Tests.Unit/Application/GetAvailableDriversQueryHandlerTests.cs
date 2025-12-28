using Bolt.Application.Features.Users.Queries.GetAvailableDrivers;
using Bolt.Domain.Entities;
using Bolt.Domain.Repositories;
using FluentAssertions;
using Moq;

namespace Bolt.Tests.Unit.Application;

public class GetAvailableDriversQueryHandlerTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly GetAvailableDriversQueryHandler _handler;

    public GetAvailableDriversQueryHandlerTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _handler = new GetAvailableDriversQueryHandler(_userRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ReturnsAvailableDrivers()
    {
        // Arrange
        var driver1 = Driver.CreateDriver(
            Guid.NewGuid(),
            "Driver One",
            "driver1@example.com",
            "+995111111111",
            "DL111111",
            "Toyota Prius",
            "TB-111-AA").Value!;

        var driver2 = Driver.CreateDriver(
            Guid.NewGuid(),
            "Driver Two",
            "driver2@example.com",
            "+995222222222",
            "DL222222",
            "Tesla Model 3",
            "TB-222-BB").Value!;

        driver2.SetAvailability(false); // Make driver2 unavailable

        var availableDrivers = new List<Driver> { driver1 };

        _userRepositoryMock
            .Setup(r => r.GetAvailableDriversAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(availableDrivers);

        var query = new GetAvailableDriversQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
        result.Value[0].FullName.Should().Be("Driver One");
        result.Value[0].IsAvailable.Should().BeTrue();

        _userRepositoryMock.Verify(
            r => r.GetAvailableDriversAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ReturnsEmptyList_WhenNoAvailableDrivers()
    {
        // Arrange
        _userRepositoryMock
            .Setup(r => r.GetAvailableDriversAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Driver>());

        var query = new GetAvailableDriversQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }
}
