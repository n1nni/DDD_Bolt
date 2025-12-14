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
        Console.WriteLine($"[DEBUG] Creating ride for passenger: {request.PassengerId}");

        var passenger = await _userRepository.GetPassengerByIdAsync(
            request.PassengerId,
            cancellationToken);

        if (passenger == null)
        {
            Console.WriteLine($"[DEBUG] Passenger not found: {request.PassengerId}");
            return Result.Failure<Guid>("Passenger not found.");
        }

        // Create Location objects
        var pickupLocation = new Location(request.PickupLatitude, request.PickupLongitude);
        var destinationLocation = new Location(request.DestinationLatitude, request.DestinationLongitude);

        Console.WriteLine($"[DEBUG] Created locations - Pickup: {pickupLocation}, Destination: {destinationLocation}");

        // Create Address objects
        var pickupAddress = new Address(
            request.PickupStreet,
            request.PickupCity,
            pickupLocation,
            request.PickupPostalCode);

        var destinationAddress = new Address(
            request.DestinationStreet,
            request.DestinationCity,
            destinationLocation,
            request.DestinationPostalCode);

        Console.WriteLine($"[DEBUG] Created addresses - Pickup: {pickupAddress}, Destination: {destinationAddress}");

        // Calculate estimated fare
        var estimatedFare = _pricingService.CalculateEstimatedFare(
            pickupLocation,
            destinationLocation);

        Console.WriteLine($"[DEBUG] Estimated fare: {estimatedFare}");

        var rideResult = RideOrder.Create(
            Guid.NewGuid(),
            passenger,
            pickupAddress,
            destinationAddress,
            estimatedFare);

        if (!rideResult.IsSuccess)
        {
            Console.WriteLine($"[DEBUG] Failed to create ride: {rideResult.Error}");
            return Result.Failure<Guid>(rideResult.Error!);
        }

        Console.WriteLine($"[DEBUG] Successfully created ride: {rideResult.Value!.Id}");

        await _rideRepository.AddAsync(rideResult.Value!, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        Console.WriteLine($"[DEBUG] Ride saved to database: {rideResult.Value!.Id}");

        return Result.Success(rideResult.Value!.Id);
    }
}