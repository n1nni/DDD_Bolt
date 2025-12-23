using FluentValidation;

namespace Bolt.Application.Features.Users.Commands.CreateDriver;

public class CreateDriverCommandValidator : AbstractValidator<CreateDriverCommand>
{
    public CreateDriverCommandValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Full name is required.")
            .MaximumLength(100).WithMessage("Full name cannot exceed 100 characters.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Phone number is required.")
            .Matches(@"^\+?[1-9]\d{1,14}$").WithMessage("Invalid phone number format.");

        RuleFor(x => x.LicenseNumber)
            .NotEmpty().WithMessage("License number is required.");

        RuleFor(x => x.VehicleModel)
            .NotEmpty().WithMessage("Vehicle model is required.");

        RuleFor(x => x.VehiclePlateNumber)
            .NotEmpty().WithMessage("Vehicle plate number is required.");
    }
}
