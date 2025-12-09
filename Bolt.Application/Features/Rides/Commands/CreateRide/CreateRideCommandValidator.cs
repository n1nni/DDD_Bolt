using FluentValidation;

namespace Bolt.Application.Features.Rides.Commands.CreateRide;

public class CreateRideCommandValidator : AbstractValidator<CreateRideCommand>
{
    public CreateRideCommandValidator()
    {
        RuleFor(x => x.PassengerId)
            .NotEmpty().WithMessage("Passenger ID is required.");

        RuleFor(x => x.PickupStreet)
            .NotEmpty().WithMessage("Pickup street is required.");

        RuleFor(x => x.PickupCity)
            .NotEmpty().WithMessage("Pickup city is required.");

        RuleFor(x => x.PickupLatitude)
            .InclusiveBetween(-90, 90).WithMessage("Invalid pickup latitude.");

        RuleFor(x => x.PickupLongitude)
            .InclusiveBetween(-180, 180).WithMessage("Invalid pickup longitude.");

        RuleFor(x => x.DestinationStreet)
            .NotEmpty().WithMessage("Destination street is required.");

        RuleFor(x => x.DestinationCity)
            .NotEmpty().WithMessage("Destination city is required.");

        RuleFor(x => x.DestinationLatitude)
            .InclusiveBetween(-90, 90).WithMessage("Invalid destination latitude.");

        RuleFor(x => x.DestinationLongitude)
            .InclusiveBetween(-180, 180).WithMessage("Invalid destination longitude.");
    }
}
