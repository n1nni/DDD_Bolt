using Bolt.Domain.Entities;
using Bolt.Domain.Enums;
using Bolt.Domain.Events;
using FluentAssertions;

namespace Bolt.Tests.Unit.Domain.Entities;

public class UserTests
{
    private readonly Guid _userId = Guid.NewGuid();
    private const string FullName = "John Doe";
    private const string Email = "john@example.com";
    private const string PhoneNumber = "+995123456789";

    [Fact]
    public void Create_WithValidParameters_ReturnsSuccessResult()
    {
        // Act
        var result = User.Create(_userId, FullName, Email, PhoneNumber, UserRole.Passenger);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Id.Should().Be(_userId);
        result.Value.FullName.Should().Be(FullName.Trim());
        result.Value.Email.Should().Be(Email.Trim().ToLowerInvariant());
        result.Value.PhoneNumber.Should().Be(PhoneNumber.Trim());
        result.Value.Role.Should().Be(UserRole.Passenger);
        result.Value.DomainEvents.Should().ContainSingle(e => e is UserCreatedEvent);
    }

    [Theory]
    [InlineData(UserRole.Passenger, typeof(Passenger))]
    [InlineData(UserRole.Driver, typeof(Driver))]
    public void Create_WithDifferentRoles_CreatesCorrectType(UserRole role, Type expectedType)
    {
        // Act
        var result = User.Create(_userId, FullName, Email, PhoneNumber, role);

        // Assert
        result.Value.Should().BeOfType(expectedType);
    }

    [Fact]
    public void Create_WithEmptyId_ReturnsFailureResult()
    {
        // Act
        var result = User.Create(Guid.Empty, FullName, Email, PhoneNumber, UserRole.Passenger);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("User ID cannot be empty.");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Create_WithEmptyFullName_ReturnsFailureResult(string fullName)
    {
        // Act
        var result = User.Create(_userId, fullName, Email, PhoneNumber, UserRole.Passenger);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Full name is required.");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Create_WithEmptyEmail_ReturnsFailureResult(string email)
    {
        // Act
        var result = User.Create(_userId, FullName, email, PhoneNumber, UserRole.Passenger);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Email is required.");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Create_WithEmptyPhoneNumber_ReturnsFailureResult(string phoneNumber)
    {
        // Act
        var result = User.Create(_userId, FullName, Email, phoneNumber, UserRole.Passenger);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Phone number is required.");
    }

    [Fact]
    public void Create_TrimsAndNormalizesInputs()
    {
        // Arrange
        const string nameWithSpaces = "  John Doe  ";
        const string emailWithSpaces = "  JOHN@EXAMPLE.COM  ";
        const string phoneWithSpaces = "  +995123456789  ";

        // Act
        var result = User.Create(_userId, nameWithSpaces, emailWithSpaces, phoneWithSpaces, UserRole.Passenger);

        // Assert
        result.Value!.FullName.Should().Be("John Doe");
        result.Value.Email.Should().Be("john@example.com");
        result.Value.PhoneNumber.Should().Be("+995123456789");
    }
}
