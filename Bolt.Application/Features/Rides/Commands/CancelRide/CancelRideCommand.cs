using Bolt.Application.Common;
using MediatR;

namespace Bolt.Application.Features.Rides.Commands.CancelRide;

public record CancelRideCommand(
    Guid RideId,
    Guid CancelledBy,
    string Reason) : IRequest<Result>;
