using Bolt.Application.Common;
using Bolt.Application.DTOs.User;
using Bolt.Domain.Repositories;
using MediatR;

namespace Bolt.Application.Features.Users.Queries.GetUserById;

public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, Result<UserDto>>
{
    private readonly IUserRepository _userRepository;

    public GetUserByIdQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Result<UserDto>> Handle(
        GetUserByIdQuery request,
        CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user == null)
        {
            return Result.Failure<UserDto>("User not found.");
        }

        var dto = new UserDto(
            user.Id,
            user.FullName,
            user.Email,
            user.PhoneNumber,
            user.Role.ToString(),
            user.CreatedAt);

        return Result.Success(dto);
    }
}
