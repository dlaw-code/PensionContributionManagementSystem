using FluentValidation;
using Pension.CORE.Dtos.Request;
using PensionContributionManagementSystem.Domain.Constants;
using System;

public class ContributionDtoValidator : AbstractValidator<ContributionRequestDto>
{
    public ContributionDtoValidator()
    {
        RuleFor(x => x.MemberId)
            .NotEmpty().WithMessage("Member ID is required");

        RuleFor(x => x.ContributionType)
            .NotEmpty().WithMessage("Contribution type is required")
            .Must(type => type == ContributionEnum.Monthly.ToString() || type == ContributionEnum.Voluntary.ToString())
            .WithMessage("Contribution type must be either 'Monthly' or 'Voluntary'");

        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("Contribution amount must be greater than 0");

        RuleFor(x => x.ContributionDate)
            .NotEmpty().WithMessage("Contribution date is required");

        RuleFor(x => x.ReferenceNumber)
            .Matches(@"^[A-Z0-9]{10}$").WithMessage("Reference number must be 10 alphanumeric characters");
    }

}
