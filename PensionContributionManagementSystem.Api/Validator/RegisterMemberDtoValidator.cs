using FluentValidation;
using PensionContributionManagementSystem.Core.Dtos.Request;
using System;

public class RegisterMemberDtoValidator : AbstractValidator<RegisterUserRequestDto>
{
    public RegisterMemberDtoValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format");

        RuleFor(x => x.DateOfBirth)
            .NotEmpty().WithMessage("Date of birth is required")
            .Must(BeValidAge).WithMessage("Age must be between 18 and 70 years");

        RuleFor(x => x.EmployerId)
            .NotEmpty().WithMessage("Employer ID is required");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters long");
    }

    private bool BeValidAge(DateTime dateOfBirth)
    {
        var age = DateTime.Today.Year - dateOfBirth.Year;
        return age >= 18 && age <= 70;
    }
}
