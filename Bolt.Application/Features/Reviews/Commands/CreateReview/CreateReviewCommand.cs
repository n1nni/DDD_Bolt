using Bolt.Application.Common;
using MediatR;

namespace Bolt.Application.Features.Reviews.Commands.CreateReview;

public record CreateReviewCommand(
    Guid RideId,
    Guid PassengerId,
    Guid DriverId,
    double RatingValue,
    string Comment) : IRequest<Result<Guid>>;