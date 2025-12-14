using Bolt.Application.Features.Reviews.Commands.CreateReview;
using Bolt.Application.Features.Reviews.Queries.GetDriverReviews;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Bolt.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReviewsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ReviewsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> CreateReview(CreateReviewCommand command)
    {
        var result = await _mediator.Send(command);

        if (result.IsSuccess)
            return Ok(new { ReviewId = result.Value });

        return BadRequest(new { Error = result.Error });
    }

    [HttpGet("driver/{driverId}")]
    public async Task<IActionResult> GetDriverReviews(Guid driverId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var query = new GetDriverReviewsQuery(driverId, page, pageSize);
        var result = await _mediator.Send(query);

        if (result.IsSuccess)
            return Ok(result.Value);

        return BadRequest(new { Error = result.Error });
    }
}
