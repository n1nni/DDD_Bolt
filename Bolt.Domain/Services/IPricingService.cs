using Bolt.Domain.ValueObjects;

namespace Bolt.Domain.Services
{
    public interface IPricingService
    {
        /// <summary>
        /// Calculates the estimated fare for a ride based on pickup and destination.
        /// </summary>
        Money CalculateEstimatedFare(Location pickup, Location destination);

        /// <summary>
        /// Calculates the final fare considering actual distance, time, and surge pricing.
        /// </summary>
        Money CalculateFinalFare(Location pickup, Location destination, TimeSpan rideDuration, bool isSurgePricing = false);
    }
}
