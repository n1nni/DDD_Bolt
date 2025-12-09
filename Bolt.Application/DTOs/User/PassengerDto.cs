namespace Bolt.Application.DTOs.User;

public record PassengerDto(
    Guid Id,
    string FullName,
    string Email,
    string PhoneNumber,
    string Role,
    DateTime CreatedAt,
    double? Rating,
    int? TotalReviews,
    string? PreferredPaymentMethod
);
