using Bolt.Application.Common;
using Bolt.Application.DTOs.Review;
using Bolt.Domain.Repositories;
using MediatR;

namespace Bolt.Application.Features.Reviews.Queries.GetDriverReviews;

public class GetDriverReviewsQueryHandler
    : IRequestHandler<GetDriverReviewsQuery, Result<PagedResult<ReviewDto>>>
{
    private readonly IReviewRepository _reviewRepository;

    public GetDriverReviewsQueryHandler(IReviewRepository reviewRepository)
    {
        _reviewRepository = reviewRepository;
    }

    public async Task<Result<PagedResult<ReviewDto>>> Handle(
        GetDriverReviewsQuery request,
        CancellationToken cancellationToken)
    {
        var reviews = await _reviewRepository.GetByDriverIdAsync(
            request.DriverId,
            request.Page,
            request.PageSize,
            cancellationToken);

        var reviewDtos = reviews.Select(r => new ReviewDto(
            r.Id,
            r.RideId,
            r.DriverId,
            r.PassengerId,
            r.Rating.Value,
            r.Comment,
            r.CreatedAt)).ToList();

        var pagedResult = new PagedResult<ReviewDto>(
            reviewDtos,
            request.Page,
            request.PageSize,
            reviewDtos.Count);

        return Result.Success(pagedResult);
    }
}
