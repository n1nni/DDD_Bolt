using Bolt.Application.Common;
using MediatR;

namespace Bolt.Application.Features.Users.Commands.CreatePassenger;

public record CreatePassengerCommand(
    string FullName,
    string Email,
    string PhoneNumber) : IRequest<Result<Guid>>;
