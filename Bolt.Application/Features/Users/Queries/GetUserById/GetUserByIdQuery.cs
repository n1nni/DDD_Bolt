using Bolt.Application.Common;
using Bolt.Application.DTOs.User;
using MediatR;

namespace Bolt.Application.Features.Users.Queries.GetUserById;

public record GetUserByIdQuery(Guid UserId) : IRequest<Result<UserDto>>;
