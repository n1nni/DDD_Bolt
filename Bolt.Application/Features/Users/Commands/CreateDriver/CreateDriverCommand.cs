using Bolt.Application.Common;
using MediatR;

public record CreateDriverCommand(
    string FullName,
    string Email,
    string PhoneNumber,
    string LicenseNumber,
    string VehicleModel,
    string VehiclePlateNumber) : IRequest<Result<Guid>>;