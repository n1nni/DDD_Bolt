using Bolt.Application.Common;
using Bolt.Application.DTOs.User;
using Bolt.Domain.Repositories;
using MediatR;

namespace Bolt.Application.Features.Users.Queries.GetAvailableDrivers;

public class GetAvailableDriversQueryHandler
    : IRequestHandler<GetAvailableDriversQuery, Result<List<DriverDto>>>
{
    private readonly IUserRepository _userRepository;

    public GetAvailableDriversQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Result<List<DriverDto>>> Handle(
        GetAvailableDriversQuery request,
        CancellationToken cancellationToken)
    {
        var drivers = await _userRepository.GetAvailableDriversAsync(cancellationToken);

        var dtos = drivers.Select(d => new DriverDto(
            d.Id,
            d.FullName,
            d.Email,
            d.PhoneNumber,
            d.Role.ToString(),
            d.CreatedAt,
            d.LicenseNumber,
            d.VehicleModel,
            d.VehiclePlateNumber,
            d.IsAvailable,
            d.Rating?.Value,
            d.Rating?.TotalReviews)).ToList();

        return Result.Success(dtos);
    }
}
