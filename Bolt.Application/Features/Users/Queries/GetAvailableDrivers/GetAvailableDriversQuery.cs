using Bolt.Application.Common;
using Bolt.Application.DTOs.User;
using MediatR;

namespace Bolt.Application.Features.Users.Queries.GetAvailableDrivers;

public record GetAvailableDriversQuery : IRequest<Result<List<DriverDto>>>;

