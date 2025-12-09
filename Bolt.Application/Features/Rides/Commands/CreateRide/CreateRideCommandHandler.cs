using Bolt.Application.Abstractions;
using Bolt.Application.Common;
using Bolt.Domain.Entities;
using Bolt.Domain.Repositories;
using Bolt.Domain.Services;
using Bolt.Domain.ValueObjects;
using MediatR;

namespace Bolt.Application.Features.Rides.Commands.CreateRide;

public class CreateRideCommandHandler : IRequestHandler<CreateRideCommand, Result<Guid>>
{
    private readonly IRideOrderRepository _rideRepository;
    private readonly IUserRepository _userRepository;
    private readonly IPricingService _pricingService;
    private readonly IUnitOfWork _unitOfWork;

    public CreateRideCommandHandler(
        IRideOrderRepository rideRepository,
        IUserRepository userRepository,
        IPricingService pricingService,
        IUnitOfWork unitOfWork)
    {
        _rideRepository = rideRepository;
        _userRepository = userRepository;
        _pricingService = pricingService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(
        CreateRideCommand request,
        CancellationToken cancellationToken)
    {
        var passenger = await _userRepository.GetPassengerByIdAsync(
            request.PassengerId,
            cancellationToken);

        if (passenger == null)
        {
            return Result.Failure<Guid>("Passenger not found.");
        }

        var pickupLocation = new Location(request.PickupLatitude, request.PickupLongitude);
        var pickupAddress = new Address(
            request.PickupStreet,
            request.PickupCity,
            pickupLocation,
            request.PickupPostalCode);

        var destinationLocation = new Location(
            request.DestinationLatitude,
            request.DestinationLongitude);
        var destinationAddress = new Address(
            request.DestinationStreet,
            request.DestinationCity,
            destinationLocation,
            request.DestinationPostalCode);

        var estimatedFare = _pricingService.CalculateEstimatedFare(
            pickupLocation,
            destinationLocation);

        var rideResult = RideOrder.Create(
            Guid.NewGuid(),
            passenger,
            pickupAddress,
            destinationAddress,
            estimatedFare);

        if (!rideResult.IsSuccess)
        {
            return Result.Failure<Guid>(rideResult.Error!);
        }

        await _rideRepository.AddAsync(rideResult.Value!, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(rideResult.Value!.Id);
    }
}
