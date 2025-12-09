using Bolt.Application.Common;
using MediatR;

namespace Bolt.Application.Features.Rides.Commands.StartRide;

public record StartRideCommand(Guid RideId) : IRequest<Result>;

