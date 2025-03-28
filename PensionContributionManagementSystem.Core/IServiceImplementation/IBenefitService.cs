using PensionContributionManagementSystem.Core.Dtos;
using PensionContributionManagementSystem.Core.Dtos.Response;
using PensionContributionManagementSystem.Domain.Entities;
namespace PensionContributionManagementSystem.Core.Abstractions
{
    public interface IBenefitService
    {
        Task<Result<BenefitResponseDto>> CalculateBenefit(string memberId);

        Task UpdateEligibilityStatus();
    }
}
