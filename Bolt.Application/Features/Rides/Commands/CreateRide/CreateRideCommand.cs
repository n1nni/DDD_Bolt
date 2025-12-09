using Bolt.Application.Common;
using MediatR;

namespace Bolt.Application.Features.Rides.Commands.CreateRide;

public record CreateRideCommand(
    Guid PassengerId,
    string PickupStreet,
    string PickupCity,
    string? PickupPostalCode,
    double PickupLatitude,
    double PickupLongitude,
    string DestinationStreet,
    string DestinationCity,
    string? DestinationPostalCode,
    double DestinationLatitude,
    double DestinationLongitude) : IRequest<Result<Guid>>;
