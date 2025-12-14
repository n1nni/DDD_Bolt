using Bolt.Application.Features.Users.Commands.CreatePassenger;
using Bolt.Application.Features.Users.Commands.UpdateDriverAvailibility;
using Bolt.Application.Features.Users.Queries.GetAvailableDrivers;
using Bolt.Application.Features.Users.Queries.GetUserById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Bolt.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUser(Guid id)
    {
        var query = new GetUserByIdQuery(id);
        var result = await _mediator.Send(query);

        if (result.IsSuccess)
            return Ok(result.Value);

        return NotFound(new { Error = result.Error });
    }

    [HttpPost("driver")]
    public async Task<IActionResult> CreateDriver(CreateDriverCommand command)
    {
        var result = await _mediator.Send(command);

        if (result.IsSuccess)
            return Ok(new { DriverId = result.Value });

        return BadRequest(new { Error = result.Error });
    }

    [HttpPost("passenger")]
    public async Task<IActionResult> CreatePassenger(CreatePassengerCommand command)
    {
        var result = await _mediator.Send(command);

        if (result.IsSuccess)
            return Ok(new { PassengerId = result.Value });

        return BadRequest(new { Error = result.Error });
    }

    [HttpPut("driver/{driverId}/availability")]
    public async Task<IActionResult> UpdateDriverAvailability(Guid driverId, UpdateDriverAvailabilityCommand command)
    {
        if (driverId != command.DriverId)
            return BadRequest("Driver ID mismatch");

        var result = await _mediator.Send(command);

        if (result.IsSuccess)
            return Ok();

        return BadRequest(new { Error = result.Error });
    }

    [HttpGet("drivers/available")]
    public async Task<IActionResult> GetAvailableDrivers()
    {
        var query = new GetAvailableDriversQuery();
        var result = await _mediator.Send(query);

        if (result.IsSuccess)
            return Ok(result.Value);

        return BadRequest(new { Error = result.Error });
    }
}