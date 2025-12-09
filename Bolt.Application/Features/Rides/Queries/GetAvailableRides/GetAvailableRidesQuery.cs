using Bolt.Application.Common;
using Bolt.Application.DTOs.Ride;
using MediatR;

namespace Bolt.Application.Features.Rides.Queries.GetAvailableRides;

public record GetAvailableRidesQuery : IRequest<Result<List<RideDto>>>;

