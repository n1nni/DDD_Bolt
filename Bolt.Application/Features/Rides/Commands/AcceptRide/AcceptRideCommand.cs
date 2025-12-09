using Bolt.Application.Common;
using MediatR;

namespace Bolt.Application.Features.Rides.Commands.AcceptRide;

public record AcceptRideCommand(
    Guid RideId,
    Guid DriverId) : IRequest<Result>;
