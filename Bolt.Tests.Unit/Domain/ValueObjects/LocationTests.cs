using Bolt.Domain.ValueObjects;
using FluentAssertions;

namespace Bolt.Tests.Unit.Domain.ValueObjects;

public class LocationTests
{
    [Theory]
    [InlineData(41.6938, 44.8015)] // Tbilisi
    [InlineData(0, 0)] // Equator
    [InlineData(-33.8688, 151.2093)] // Sydney
    public void Constructor_WithValidCoordinates_CreatesLocation(double latitude, double longitude)
    {
        // Act
        var location = new Location(latitude, longitude);

        // Assert
        location.Latitude.Should().Be(latitude);
        location.Longitude.Should().Be(longitude);
    }

    [Theory]
    [InlineData(-91, 0)]
    [InlineData(91, 0)]
    [InlineData(0, -181)]
    [InlineData(0, 181)]
    public void Constructor_WithInvalidCoordinates_ThrowsException(double latitude, double longitude)
    {
        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => new Location(latitude, longitude));
    }

    [Fact]
    public void CalculateDistanceTo_ReturnsCorrectDistance()
    {
        // Arrange
        var location1 = new Location(41.6938, 44.8015); // Tbilisi
        var location2 = new Location(41.7167, 44.7833); // Rustavi (approx 30km)

        // Act
        var distance = location1.CalculateDistanceTo(location2);

        // Assert - Should be around 3 km, not 30 km!
        distance.Should().BeGreaterThan(2).And.BeLessThan(4); // Approx 3 km
    }

    [Theory]
    [InlineData(41.6938, 44.8015, 41.6938, 44.8015, true)]
    [InlineData(41.6938, 44.8015, 41.6938001, 44.8015001, true)] // Within tolerance
    [InlineData(41.6938, 44.8015, 42.6938, 44.8015, false)]
    public void Equals_ComparesCoordinatesWithTolerance(double lat1, double lon1, double lat2, double lon2, bool expected)
    {
        // Arrange
        var location1 = new Location(lat1, lon1);
        var location2 = new Location(lat2, lon2);

        // Act & Assert
        location1.Equals(location2).Should().Be(expected);
    }
}
