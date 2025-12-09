using FluentValidation;

namespace Bolt.Application.Features.Reviews.Commands.CreateReview;

public class CreateReviewCommandValidator : AbstractValidator<CreateReviewCommand>
{
    public CreateReviewCommandValidator()
    {
        RuleFor(x => x.RideId)
            .NotEmpty().WithMessage("Ride ID is required.");

        RuleFor(x => x.PassengerId)
            .NotEmpty().WithMessage("Passenger ID is required.");

        RuleFor(x => x.DriverId)
            .NotEmpty().WithMessage("Driver ID is required.");

        RuleFor(x => x.RatingValue)
            .InclusiveBetween(0.0, 5.0).WithMessage("Rating must be between 0 and 5.");

        RuleFor(x => x.Comment)
            .MaximumLength(500).WithMessage("Comment cannot exceed 500 characters.");
    }
}
