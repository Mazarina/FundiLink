using FluentValidation;

namespace FundiLink.Application.Features.Auth.Commands.RegisterLearner;

public class RegisterLearnerValidator : AbstractValidator<RegisterLearnerCommand>
{
    public RegisterLearnerValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty().MinimumLength(8);
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Surname).NotEmpty().MaximumLength(100);
        RuleFor(x => x.DateOfBirth).NotEmpty()
            .Must(dob => dob < DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-10)))
            .WithMessage("Date of birth is not valid.");
        RuleFor(x => x.MobileNumber).NotEmpty().MaximumLength(20);
        RuleFor(x => x.Province).NotEmpty().MaximumLength(50);
        RuleFor(x => x.SchoolName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.SchoolProvince).NotEmpty().MaximumLength(50);
        RuleFor(x => x.ConsentAccepted).Equal(true)
            .WithMessage("POPIA consent must be accepted to register.");
    }
}
