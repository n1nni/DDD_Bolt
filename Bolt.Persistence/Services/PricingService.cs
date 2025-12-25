using Bolt.Domain.Services;
using Bolt.Domain.ValueObjects;

namespace Bolt.Persistence.Services;

public class PricingService : IPricingService
{
    private const decimal BaseFare = 2.50m;
    private const decimal PerKilometerRate = 1.50m;
    private const decimal PerMinuteRate = 0.25m;
    private const decimal SurgeMultiplier = 1.5m;

    public Money CalculateEstimatedFare(Location pickup, Location destination)
    {
        var distanceKm = (decimal)pickup.CalculateDistanceTo(destination); // Cast to decimal
        var estimatedTimeMinutes = distanceKm * 2m; // Use decimal literal

        var distanceCost = distanceKm * PerKilometerRate;
        var timeCost = estimatedTimeMinutes * PerMinuteRate;
        var total = BaseFare + distanceCost + timeCost;

        // Round up to nearest 0.50
        total = Math.Ceiling(total * 2m) / 2m;

        return new Money(total, "GEL");
    }

    public Money CalculateFinalFare(Location pickup, Location destination, TimeSpan rideDuration, bool isSurgePricing = false)
    {
        var distanceKm = (decimal)pickup.CalculateDistanceTo(destination); // Cast to decimal
        var durationMinutes = (decimal)rideDuration.TotalMinutes;

        var distanceCost = distanceKm * PerKilometerRate;
        var timeCost = durationMinutes * PerMinuteRate;
        var total = BaseFare + distanceCost + timeCost;

        if (isSurgePricing)
        {
            total *= SurgeMultiplier;
        }

        // Round up to nearest 0.25
        total = Math.Ceiling(total * 4m) / 4m;


        return new Money(total, "GEL");
    }
}

