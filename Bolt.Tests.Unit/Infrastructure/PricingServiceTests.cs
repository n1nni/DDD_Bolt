using Bolt.Domain.ValueObjects;
using Bolt.Persistence.Services;
using FluentAssertions;

namespace Bolt.Tests.Unit.Infrastructure;

public class PricingServiceTests
{
    private readonly PricingService _pricingService;

    public PricingServiceTests()
    {
        _pricingService = new PricingService();
    }

    [Fact]
    public void CalculateEstimatedFare_WithValidLocations_ReturnsFare()
    {
        // Arrange
        var pickup = new Location(41.6938, 44.8015); // Tbilisi
        var destination = new Location(41.7167, 44.7833); // Rustavi (~3km)

        // Act
        var fare = _pricingService.CalculateEstimatedFare(pickup, destination);

        // Assert
        fare.Should().NotBeNull();
        fare.Currency.Should().Be("GEL");
        fare.Amount.Should().BeGreaterThan(0);

        // Base fare (2.50) + distance (~3km * 1.50 = 4.50) + time (~6min * 0.25 = 1.50)
        // Total ≈ 8.50, rounded up to nearest 0.50 = 9.00
        fare.Amount.Should().BeApproximately(9.00m, 2.00m);
    }

    [Fact]
    public void CalculateFinalFare_WithValidParameters_ReturnsFare()
    {
        // Arrange
        var pickup = new Location(41.6938, 44.8015);
        var destination = new Location(41.7167, 44.7833);
        var rideDuration = TimeSpan.FromMinutes(10);

        // Act
        var fare = _pricingService.CalculateFinalFare(pickup, destination, rideDuration, false);

        // Assert
        fare.Should().NotBeNull();
        fare.Currency.Should().Be("GEL");
        fare.Amount.Should().BeGreaterThan(0);
    }

    [Fact]
    public void CalculateFinalFare_WithSurgePricing_ReturnsHigherFare()
    {
        // Arrange
        var pickup = new Location(41.6938, 44.8015);
        var destination = new Location(41.7167, 44.7833);
        var rideDuration = TimeSpan.FromMinutes(10);

        // Act
        var normalFare = _pricingService.CalculateFinalFare(pickup, destination, rideDuration, false);
        var surgeFare = _pricingService.CalculateFinalFare(pickup, destination, rideDuration, true);

        // Assert
        surgeFare.Amount.Should().BeGreaterThan(normalFare.Amount);
        surgeFare.Amount.Should().BeApproximately(normalFare.Amount * 1.5m, 0.1m);
    }

    [Fact]
    public void CalculateEstimatedFare_SameLocation_ReturnsBaseFare()
    {
        // Arrange
        var location = new Location(41.6938, 44.8015);

        // Act
        var fare = _pricingService.CalculateEstimatedFare(location, location);

        // Assert
        fare.Amount.Should().Be(2.50m); // Base fare rounded up to nearest 0.50
    }
}
