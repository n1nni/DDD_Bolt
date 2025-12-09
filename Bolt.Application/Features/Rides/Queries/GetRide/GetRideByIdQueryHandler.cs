using Bolt.Application.Common;
using Bolt.Application.DTOs.Ride;
using Bolt.Domain.Repositories;
using MediatR;

namespace Bolt.Application.Features.Rides.Queries.GetRide;

public class GetRideByIdQueryHandler : IRequestHandler<GetRideByIdQuery, Result<RideDto>>
{
    private readonly IRideOrderRepository _rideRepository;

    public GetRideByIdQueryHandler(IRideOrderRepository rideRepository)
    {
        _rideRepository = rideRepository;
    }

    public async Task<Result<RideDto>> Handle(
        GetRideByIdQuery request,
        CancellationToken cancellationToken)
    {
        var ride = await _rideRepository.GetByIdAsync(request.RideId, cancellationToken);
        if (ride == null)
        {
            return Result.Failure<RideDto>("Ride not found.");
        }

        var dto = new RideDto(
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
            ride.CompletedAt);

        return Result.Success(dto);
    }
}