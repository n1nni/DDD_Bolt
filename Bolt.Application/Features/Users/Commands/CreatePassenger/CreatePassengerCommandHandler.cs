using Bolt.Application.Abstractions;
using Bolt.Application.Common;
using Bolt.Domain.Entities;
using Bolt.Domain.Enums;
using Bolt.Domain.Repositories;
using MediatR;

namespace Bolt.Application.Features.Users.Commands.CreatePassenger;

public class CreatePassengerCommandHandler : IRequestHandler<CreatePassengerCommand, Result<Guid>>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreatePassengerCommandHandler(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(
        CreatePassengerCommand request,
        CancellationToken cancellationToken)
    {
        // Check if email already exists
        var existingUser = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (existingUser != null)
        {
            return Result.Failure<Guid>("A user with this email already exists.");
        }

        // Create passenger
        var userResult = User.Create(
            Guid.NewGuid(),
            request.FullName,
            request.Email,
            request.PhoneNumber,
            UserRole.Passenger);

        if (!userResult.IsSuccess)
        {
            return Result.Failure<Guid>(userResult.Error!);
        }

        await _userRepository.AddAsync(userResult.Value!, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(userResult.Value!.Id);
    }
}
