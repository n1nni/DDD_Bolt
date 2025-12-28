using Bolt.Application.Abstractions;
using Bolt.Application.Features.Users.Commands.CreateDriver;
using Bolt.Domain.Entities;
using Bolt.Domain.Repositories;
using FluentAssertions;
using Moq;

namespace Bolt.Tests.Unit.Application;

public class CreateDriverCommandHandlerTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly CreateDriverCommandHandler _handler;

    public CreateDriverCommandHandlerTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _handler = new CreateDriverCommandHandler(
            _userRepositoryMock.Object,
            _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidCommand_ReturnsSuccess()
    {
        // Arrange
        var command = new CreateDriverCommand(
            "John Driver",
            "john.driver@example.com",
            "+995123456789",
            "DL123456",
            "Toyota Prius",
            "TB-123-AA");

        _userRepositoryMock
            .Setup(r => r.GetByEmailAsync(command.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        _userRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBe(Guid.Empty);

        _userRepositoryMock.Verify(
            r => r.GetByEmailAsync(command.Email, It.IsAny<CancellationToken>()),
            Times.Once);

        _userRepositoryMock.Verify(
            r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()),
            Times.Once);

        _unitOfWorkMock.Verify(
            u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WithExistingEmail_ReturnsFailure()
    {
        // Arrange
        var command = new CreateDriverCommand(
            "John Driver",
            "existing@example.com",
            "+995123456789",
            "DL123456",
            "Toyota Prius",
            "TB-123-AA");

        var existingUser = User.Create(
            Guid.NewGuid(),
            "Existing User",
            "existing@example.com",
            "+995123456789",
            Bolt.Domain.Enums.UserRole.Driver).Value;

        _userRepositoryMock
            .Setup(r => r.GetByEmailAsync(command.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingUser);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("A user with this email already exists.");

        _userRepositoryMock.Verify(
            r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_WithInvalidCommand_ReturnsFailure()
    {
        // Arrange
        var command = new CreateDriverCommand(
            "",  // Empty name
            "invalid-email",
            "",  // Empty phone
            "",  // Empty license
            "",  // Empty model
            ""); // Empty plate

        _userRepositoryMock
            .Setup(r => r.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("is required"); // From domain validation

        _userRepositoryMock.Verify(
            r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }
}