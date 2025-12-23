using Bolt.Domain.Enums;
using Bolt.Domain.ValueObjects;

namespace Bolt.Domain.Entities;

public sealed class Passenger : User
{
    public Rating? Rating { get; private set; }
    public string? PreferredPaymentMethod { get; private set; }

    private readonly List<Guid> _rideHistoryIds = new();
    public IReadOnlyCollection<Guid> RideHistoryIds => _rideHistoryIds.AsReadOnly();

    private Passenger() { } // EF Core

    internal Passenger(Guid id, string fullName, string email, string phoneNumber)
        : base(id, fullName, email, phoneNumber, UserRole.Passenger)
    {
    }

    public void AddRideToHistory(Guid rideId)
    {
        if (rideId == Guid.Empty)
            throw new ArgumentException("Ride ID cannot be empty.", nameof(rideId));

        if (!_rideHistoryIds.Contains(rideId))
        {
            _rideHistoryIds.Add(rideId);
            Console.WriteLine($"[LOG] Ride added to passenger history: Passenger {Id}, Ride {rideId}");
        }
    }
}
