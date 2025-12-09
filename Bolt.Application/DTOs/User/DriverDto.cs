namespace Bolt.Application.DTOs.User;

public record DriverDto(
    Guid Id,
    string FullName,
    string Email,
    string PhoneNumber,
    string Role,
    DateTime CreatedAt,
    string LicenseNumber,
    string VehicleModel,
    string VehiclePlateNumber,
    bool IsAvailable,
    double? Rating,
    int? TotalReviews
);