using Bolt.Application.Abstractions;
using Bolt.Application.Common;
using Bolt.Domain.Entities;
using Bolt.Domain.Repositories;
using MediatR;

namespace Bolt.Application.Features.Users.Commands.CreateDriver;

public class CreateDriverCommandHandler : IRequestHandler<CreateDriverCommand, Result<Guid>>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateDriverCommandHandler(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(
        CreateDriverCommand request,
        CancellationToken cancellationToken)
    {
        var existingUser = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (existingUser != null)
        {
            return Result.Failure<Guid>("A user with this email already exists.");
        }

        var driverResult = Driver.CreateDriver(
            Guid.NewGuid(),
            request.FullName,
            request.Email,
            request.PhoneNumber,
            request.LicenseNumber,
            request.VehicleModel,
            request.VehiclePlateNumber);

        if (!driverResult.IsSuccess)
        {
            return Result.Failure<Guid>(driverResult.Error!);
        }

        await _userRepository.AddAsync(driverResult.Value!, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(driverResult.Value!.Id);
    }
}

