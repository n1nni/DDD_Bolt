using Bolt.Application.DTOs.User;

namespace Bolt.Application.DTOs.Ride;

public record RideDetailDto(
    Guid Id,
    PassengerDto Passenger,
    DriverDto? Driver,
    AddressDto PickupAddress,
    AddressDto DestinationAddress,
    decimal EstimatedFare,
    string Currency,
    decimal? FinalFare,
    string Status,
    DateTime CreatedAt,
    DateTime? AcceptedAt,
    DateTime? StartedAt,
    DateTime? CompletedAt,
    string? CancellationReason
);
