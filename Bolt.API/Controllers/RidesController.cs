using Bolt.Application.Features.Rides.Commands.AcceptRide;
using Bolt.Application.Features.Rides.Commands.CancelRide;
using Bolt.Application.Features.Rides.Commands.CompleteRide;
using Bolt.Application.Features.Rides.Commands.CreateRide;
using Bolt.Application.Features.Rides.Commands.StartRide;
using Bolt.Application.Features.Rides.Queries.GetAvailableRides;
using Bolt.Application.Features.Rides.Queries.GetPassengerRides;
using Bolt.Application.Features.Rides.Queries.GetRide;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Bolt.API.Controllers;


[ApiController]
[Route("api/[controller]")]
public class RidesController : ControllerBase
{
    private readonly IMediator _mediator;

    public RidesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetRide(Guid id)
    {
        var query = new GetRideByIdQuery(id);
        var result = await _mediator.Send(query);

        if (result.IsSuccess)
            return Ok(result.Value);

        return NotFound(new { Error = result.Error });
    }

    [HttpPost]
    public async Task<IActionResult> CreateRide(CreateRideCommand command)
    {
        var result = await _mediator.Send(command);

        if (result.IsSuccess)
            return Ok(new { RideId = result.Value });

        return BadRequest(new { Error = result.Error });
    }

    [HttpPost("{id}/accept")]
    public async Task<IActionResult> AcceptRide(Guid id, [FromBody] AcceptRideCommand command)
    {
        if (id != command.RideId)
            return BadRequest("Ride ID mismatch");

        var result = await _mediator.Send(command);

        if (result.IsSuccess)
            return Ok();

        return BadRequest(new { Error = result.Error });
    }

    [HttpPost("{id}/start")]
    public async Task<IActionResult> StartRide(Guid id)
    {
        var command = new StartRideCommand(id);
        var result = await _mediator.Send(command);

        if (result.IsSuccess)
            return Ok();

        return BadRequest(new { Error = result.Error });
    }

    [HttpPost("{id}/complete")]
    public async Task<IActionResult> CompleteRide(Guid id, CompleteRideCommand command)
    {
        if (id != command.RideId)
            return BadRequest("Ride ID mismatch");

        var result = await _mediator.Send(command);

        if (result.IsSuccess)
            return Ok();

        return BadRequest(new { Error = result.Error });
    }

    [HttpPost("{id}/cancel")]
    public async Task<IActionResult> CancelRide(Guid id, CancelRideCommand command)
    {
        if (id != command.RideId)
            return BadRequest("Ride ID mismatch");

        var result = await _mediator.Send(command);

        if (result.IsSuccess)
            return Ok();

        return BadRequest(new { Error = result.Error });
    }

    [HttpGet("available")]
    public async Task<IActionResult> GetAvailableRides()
    {
        var query = new GetAvailableRidesQuery();
        var result = await _mediator.Send(query);

        if (result.IsSuccess)
            return Ok(result.Value);

        return BadRequest(new { Error = result.Error });
    }

    [HttpGet("passenger/{passengerId}")]
    public async Task<IActionResult> GetPassengerRides(Guid passengerId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var query = new GetPassengerRidesQuery(passengerId, page, pageSize);
        var result = await _mediator.Send(query);

        if (result.IsSuccess)
            return Ok(result.Value);

        return BadRequest(new { Error = result.Error });
    }
}
