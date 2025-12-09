using Bolt.Application.Common;
using Bolt.Application.DTOs.Review;
using MediatR;

namespace Bolt.Application.Features.Reviews.Queries.GetDriverReviews;

public record GetDriverReviewsQuery(
    Guid DriverId,
    int Page = 1,
    int PageSize = 10) : IRequest<Result<PagedResult<ReviewDto>>>;
