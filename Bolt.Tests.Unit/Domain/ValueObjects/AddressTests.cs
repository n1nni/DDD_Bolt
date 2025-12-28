using Bolt.Domain.ValueObjects;
using FluentAssertions;

namespace Bolt.Tests.Unit.Domain.ValueObjects;

public class AddressTests
{
    [Fact]
    public void Constructor_WithValidParameters_CreatesAddress()
    {
        // Arrange
        var location = new Location(41.6938, 44.8015);

        // Act
        var address = new Address("Rustaveli Ave", "Tbilisi", location, "0108");

        // Assert
        address.Street.Should().Be("Rustaveli Ave");
        address.City.Should().Be("Tbilisi");
        address.PostalCode.Should().Be("0108");
        address.Location.Should().Be(location);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Constructor_WithEmptyStreet_ThrowsException(string street)
    {
        // Arrange
        var location = new Location(41.6938, 44.8015);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new Address(street, "Tbilisi", location));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Constructor_WithEmptyCity_ThrowsException(string city)
    {
        // Arrange
        var location = new Location(41.6938, 44.8015);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new Address("Rustaveli Ave", city, location));
    }

    [Fact]
    public void Constructor_WithNullLocation_ThrowsException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new Address("Rustaveli Ave", "Tbilisi", null!));
    }

    [Fact]
    public void Equals_ComparesStreetCityAndLocation()
    {
        // Arrange
        var location = new Location(41.6938, 44.8015);
        var address1 = new Address("Rustaveli Ave", "Tbilisi", location);
        var address2 = new Address("Rustaveli Ave", "Tbilisi", location);
        var address3 = new Address("Different St", "Tbilisi", location);

        // Assert
        address1.Equals(address2).Should().BeTrue();
        address1.Equals(address3).Should().BeFalse();
    }

    [Fact]
    public void Constructor_TrimsInputStrings()
    {
        // Arrange
        var location = new Location(41.6938, 44.8015);

        // Act
        var address = new Address("  Rustaveli Ave  ", "  Tbilisi  ", location, "  0108  ");

        // Assert
        address.Street.Should().Be("Rustaveli Ave");
        address.City.Should().Be("Tbilisi");
        address.PostalCode.Should().Be("0108");
    }
}
