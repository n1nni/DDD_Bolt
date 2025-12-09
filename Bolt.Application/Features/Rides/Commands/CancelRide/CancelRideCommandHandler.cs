using Bolt.Application.Abstractions;
using Bolt.Application.Common;
using Bolt.Domain.Repositories;
using MediatR;

namespace Bolt.Application.Features.Rides.Commands.CancelRide;

public class CancelRideCommandHandler : IRequestHandler<CancelRideCommand, Result>
{
    private readonly IRideOrderRepository _rideRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CancelRideCommandHandler(
        IRideOrderRepository rideRepository,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork)
    {
        _rideRepository = rideRepository;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(
        CancelRideCommand request,
        CancellationToken cancellationToken)
    {
        var ride = await _rideRepository.GetByIdAsync(request.RideId, cancellationToken);
        if (ride == null)
        {
            return Result.Failure("Ride not found.");
        }

        var cancelResult = ride.Cancel(request.CancelledBy, request.Reason);
        if (!cancelResult.IsSuccess)
        {
            return Result.Failure(cancelResult.Error!);
        }

        // If driver was assigned, make them available again
        if (ride.DriverId.HasValue)
        {
            var driver = await _userRepository.GetDriverByIdAsync(
                ride.DriverId.Value,
                cancellationToken);

            if (driver != null)
            {
                driver.SetAvailability(true);
                _userRepository.Update(driver);
            }
        }

        _rideRepository.Update(ride);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
