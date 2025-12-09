using Bolt.Application.Abstractions;
using Bolt.Application.Common;
using Bolt.Domain.Repositories;
using MediatR;

namespace Bolt.Application.Features.Users.Commands.UpdateDriverAvailibility;

public class UpdateDriverAvailabilityCommandHandler
    : IRequestHandler<UpdateDriverAvailabilityCommand, Result>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateDriverAvailabilityCommandHandler(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(
        UpdateDriverAvailabilityCommand request,
        CancellationToken cancellationToken)
    {
        var driver = await _userRepository.GetDriverByIdAsync(request.DriverId, cancellationToken);
        if (driver == null)
        {
            return Result.Failure("Driver not found.");
        }

        driver.SetAvailability(request.IsAvailable);
        _userRepository.Update(driver);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
