using Bolt.Application.Abstractions;
using Bolt.Application.Common;
using Bolt.Domain.Repositories;
using MediatR;

namespace Bolt.Application.Features.Rides.Commands.AcceptRide;

public class AcceptRideCommandHandler : IRequestHandler<AcceptRideCommand, Result>
{
    private readonly IRideOrderRepository _rideRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AcceptRideCommandHandler(
        IRideOrderRepository rideRepository,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork)
    {
        _rideRepository = rideRepository;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(
        AcceptRideCommand request,
        CancellationToken cancellationToken)
    {
        var ride = await _rideRepository.GetByIdAsync(request.RideId, cancellationToken);
        if (ride == null)
        {
            return Result.Failure("Ride not found.");
        }

        var driver = await _userRepository.GetDriverByIdAsync(request.DriverId, cancellationToken);
        if (driver == null)
        {
            return Result.Failure("Driver not found.");
        }

        var acceptResult = ride.Accept(driver);
        if (!acceptResult.IsSuccess)
        {
            return Result.Failure(acceptResult.Error!);
        }

        driver.SetAvailability(false);

        _rideRepository.Update(ride);
        _userRepository.Update(driver);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
