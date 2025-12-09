using Bolt.Application.Common;
using MediatR;

namespace Bolt.Application.Features.Users.Commands.UpdateDriverAvailibility;

public record UpdateDriverAvailabilityCommand(
    Guid DriverId,
    bool IsAvailable) : IRequest<Result>;
