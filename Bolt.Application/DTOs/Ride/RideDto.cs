namespace Bolt.Application.DTOs.Ride;

public record RideDto(
    Guid Id,
    Guid PassengerId,
    Guid? DriverId,
    AddressDto PickupAddress,
    AddressDto DestinationAddress,
    decimal EstimatedFare,
    string Currency,
    decimal? FinalFare,
    string Status,
    DateTime CreatedAt,
    DateTime? AcceptedAt,
    DateTime? StartedAt,
    DateTime? CompletedAt
);