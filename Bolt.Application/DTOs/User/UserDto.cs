namespace Bolt.Application.DTOs.User;

public record UserDto(
    Guid Id,
    string FullName,
    string Email,
    string PhoneNumber,
    string Role,
    DateTime CreatedAt
);
