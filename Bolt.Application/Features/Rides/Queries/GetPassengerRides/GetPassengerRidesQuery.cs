using Bolt.Application.Common;
using Bolt.Application.DTOs.Ride;
using MediatR;

namespace Bolt.Application.Features.Rides.Queries.GetPassengerRides;

public record GetPassengerRidesQuery(
    Guid PassengerId,
    int Page = 1,
    int PageSize = 10) : IRequest<Result<PagedResult<RideDto>>>;
