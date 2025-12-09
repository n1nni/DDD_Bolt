using Bolt.Application.Common;
using Bolt.Application.DTOs.Ride;
using MediatR;

namespace Bolt.Application.Features.Rides.Queries.GetRide;

public record GetRideByIdQuery(Guid RideId) : IRequest<Result<RideDto>>;