using Bolt.Application.Common;
using Bolt.Application.DTOs.Ride;
using Bolt.Domain.Repositories;
using MediatR;

namespace Bolt.Application.Features.Rides.Queries.GetPassengerRides;

public class GetPassengerRidesQueryHandler
    : IRequestHandler<GetPassengerRidesQuery, Result<PagedResult<RideDto>>>
{
    private readonly IRideOrderRepository _rideRepository;

    public GetPassengerRidesQueryHandler(IRideOrderRepository rideRepository)
    {
        _rideRepository = rideRepository;
    }

    public async Task<Result<PagedResult<RideDto>>> Handle(
        GetPassengerRidesQuery request,
        CancellationToken cancellationToken)
    {
        var rides = await _rideRepository.GetByPassengerIdAsync(
            request.PassengerId,
            request.Page,
            request.PageSize,
            cancellationToken);

        var rideDtos = rides.Select(ride => new RideDto(
            ride.Id,
            ride.PassengerId,
            ride.DriverId,
            new AddressDto(
                ride.PickupAddress.Street,
                ride.PickupAddress.City,
                ride.PickupAddress.PostalCode,
                ride.PickupAddress.Location.Latitude,
                ride.PickupAddress.Location.Longitude),
            new AddressDto(
                ride.DestinationAddress.Street,
                ride.DestinationAddress.City,
                ride.DestinationAddress.PostalCode,
                ride.DestinationAddress.Location.Latitude,
                ride.DestinationAddress.Location.Longitude),
            ride.EstimatedFare.Amount,
            ride.EstimatedFare.Currency,
            ride.FinalFare?.Amount,
            ride.Status.ToString(),
            ride.CreatedAt,
            ride.AcceptedAt,
            ride.StartedAt,
            ride.CompletedAt)).ToList();

        var pagedResult = new PagedResult<RideDto>(
            rideDtos,
            request.Page,
            request.PageSize,
            rideDtos.Count);

        return Result.Success(pagedResult);
    }
}
