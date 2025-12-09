namespace Bolt.Application.DTOs.Review;

public record ReviewDto(
    Guid Id,
    Guid RideId,
    Guid DriverId,
    Guid PassengerId,
    double Rating,
    string Comment,
    DateTime CreatedAt
);
