using FluentValidation;
using PensionContributionManagementSystem.Core.Dtos.Request;
using System;

public class UpdateMemberDtoValidator : AbstractValidator<UpdateMemberRequestDto>
{
    public UpdateMemberDtoValidator()
    {
        RuleFor(x => x.Email)
            .EmailAddress().When(x => !string.IsNullOrEmpty(x.Email))
            .WithMessage("Invalid email format");

        RuleFor(x => x.DateOfBirth)
            .Must(BeValidAge).When(x => x.DateOfBirth.HasValue)
            .WithMessage("Age must be between 18 and 70 years");
    }

    private bool BeValidAge(DateTime? dateOfBirth)
    {
        if (!dateOfBirth.HasValue) return true;
        var age = DateTime.Today.Year - dateOfBirth.Value.Year;
        return age >= 18 && age <= 70;
    }
}
