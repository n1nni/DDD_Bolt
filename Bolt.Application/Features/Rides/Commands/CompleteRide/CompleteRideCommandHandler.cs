using Bolt.Application.Abstractions;
using Bolt.Application.Common;
using Bolt.Domain.Repositories;
using Bolt.Domain.ValueObjects;
using MediatR;

namespace Bolt.Application.Features.Rides.Commands.CompleteRide;

public class CompleteRideCommandHandler : IRequestHandler<CompleteRideCommand, Result>
{
    private readonly IRideOrderRepository _rideRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CompleteRideCommandHandler(
        IRideOrderRepository rideRepository,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork)
    {
        _rideRepository = rideRepository;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(
        CompleteRideCommand request,
        CancellationToken cancellationToken)
    {
        var ride = await _rideRepository.GetByIdAsync(request.RideId, cancellationToken);
        if (ride == null)
        {
            return Result.Failure("Ride not found.");
        }

        var finalFare = new Money(request.FinalFare, request.Currency);
        var completeResult = ride.Complete(finalFare);

        if (!completeResult.IsSuccess)
        {
            return Result.Failure(completeResult.Error!);
        }

        // Update driver availability
        if (ride.DriverId.HasValue)
        {
            var driver = await _userRepository.GetDriverByIdAsync(
                ride.DriverId.Value,
                cancellationToken);

            if (driver != null)
            {
                driver.SetAvailability(true);
                driver.AddCompletedRide(ride.Id);
                _userRepository.Update(driver);
            }
        }

        // Update passenger ride history
        var passenger = await _userRepository.GetPassengerByIdAsync(
            ride.PassengerId,
            cancellationToken);

        if (passenger != null)
        {
            passenger.AddRideToHistory(ride.Id);
            _userRepository.Update(passenger);
        }

        _rideRepository.Update(ride);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
