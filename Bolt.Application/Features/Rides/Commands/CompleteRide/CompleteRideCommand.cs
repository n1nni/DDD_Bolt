using Bolt.Application.Common;
using MediatR;

namespace Bolt.Application.Features.Rides.Commands.CompleteRide;

public record CompleteRideCommand(
    Guid RideId,
    decimal FinalFare,
    string Currency = "USD") : IRequest<Result>;