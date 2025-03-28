using FluentValidation;
using PensionContributionManagementSystem.Core.Dtos.Request;
using System.Text.RegularExpressions;

public class AddEmployerDtoValidator : AbstractValidator<AddEmployerDto>
{
    public AddEmployerDtoValidator()
    {
        RuleFor(x => x.CompanyName)
            .NotEmpty().WithMessage("Company name is required");

        RuleFor(x => x.RegistrationNumber)
            .NotEmpty().WithMessage("Registration number is required")
            .Matches(@"^[A-Z0-9-]+$").WithMessage("Invalid registration number format");
    }
}
