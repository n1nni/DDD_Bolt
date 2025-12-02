using Bolt.Domain.ValueObjects;

namespace Bolt.Domain.Services
{
    public interface IPricingService
    {
        /// <summary>
        /// Calculate an estimated fare from pickup to destination.
        /// </summary>
        Money CalculateEstimatedFare(Location pickup, Location destination);
    }
}
