namespace Bolt.Application.DTOs.Ride;

public record AddressDto(
    string Street,
    string City,
    string? PostalCode,
    double Latitude,
    double Longitude
);
