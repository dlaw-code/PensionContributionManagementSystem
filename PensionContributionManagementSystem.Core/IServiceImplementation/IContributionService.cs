using Pension.CORE.Dtos.Request;
using PensionContributionManagementSystem.Core.Dtos;
using PensionContributionManagementSystem.Core.Dtos.Response;
using PensionContributionManagementSystem.Domain.Entities;

namespace PensionContributionManagementSystem.Core.Abstractions
{
    public interface IContributionService
    {
        Task<Result<ContributionResponseDto>> AddContribution(ContributionRequestDto contribution);
        Task<Result<IEnumerable<ContributionResponseDto>>> GetMemberContributions(string memberId, int pageSize = 10, int offset = 0);

        Task<Result<IEnumerable<TransactionHistoryDto>>> GetTransactionHistoryByMemberId(string memberId, int pageSize = 10, int offset =0);
        Task CalculateMonthlyInterest();
    }

}

