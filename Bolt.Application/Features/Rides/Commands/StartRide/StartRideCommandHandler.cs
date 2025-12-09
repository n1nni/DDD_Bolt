using Bolt.Application.Abstractions;
using Bolt.Application.Common;
using Bolt.Domain.Repositories;
using MediatR;

namespace Bolt.Application.Features.Rides.Commands.StartRide;

public class StartRideCommandHandler : IRequestHandler<StartRideCommand, Result>
{
    private readonly IRideOrderRepository _rideRepository;
    private readonly IUnitOfWork _unitOfWork;

    public StartRideCommandHandler(
        IRideOrderRepository rideRepository,
        IUnitOfWork unitOfWork)
    {
        _rideRepository = rideRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(
        StartRideCommand request,
        CancellationToken cancellationToken)
    {
        var ride = await _rideRepository.GetByIdAsync(request.RideId, cancellationToken);
        if (ride == null)
        {
            return Result.Failure("Ride not found.");
        }

        var startResult = ride.Start();
        if (!startResult.IsSuccess)
        {
            return Result.Failure(startResult.Error!);
        }

        _rideRepository.Update(ride);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}