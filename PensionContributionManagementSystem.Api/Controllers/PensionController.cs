using Microsoft.AspNetCore.Mvc;
using Pension.CORE.Dtos.Request;
using PensionContributionManagementSystem.Api.Dtos;
using PensionContributionManagementSystem.Core.Abstractions;
using PensionContributionManagementSystem.Core.Dtos.Request;
using PensionContributionManagementSystem.Core.Dtos.Response;
using System.Net;
using System.Threading.Tasks;

namespace PensionContributionManagementSystem.API.Controllers
{
    [ApiController]
    [Route("api/pension")]
    public class PensionController : ControllerBase
    {
        private readonly IContributionService _contributionService;
        private readonly IBenefitService _benefitService;

        public PensionController(IContributionService contributionService, IBenefitService benefitService)
        {
            _contributionService = contributionService;
            _benefitService = benefitService;
        }

        /// <summary>
        /// Adds a new contribution.
        /// </summary>
        [HttpPost("contributions")]
        public async Task<IActionResult> AddContribution([FromBody] ContributionRequestDto contribution)
        {
            var result = await _contributionService.AddContribution(contribution);
            if (!result.IsSuccess)
                return BadRequest(ResponseDto<ContributionResponseDto>.Failure(result.Errors, (int)HttpStatusCode.BadRequest));

            return CreatedAtAction(
                nameof(GetMemberContributions),
                new { memberId = contribution.MemberId },
                ResponseDto<ContributionResponseDto>.Success(result.Data, "Contribution added successfully")
            );
        }

        /// <summary>
        /// Retrieves all contributions for a specific member.
        /// </summary>
        [HttpGet("contributions/member/{memberId}")]
        public async Task<IActionResult> GetMemberContributions(string memberId, [FromQuery] PaginationRequestDto pagination)
        {

            var result = await _contributionService.GetMemberContributions(memberId, pagination.pageSize, pagination.offset);

            if (!result.IsSuccess)
                return NotFound(ResponseDto<IEnumerable<ContributionResponseDto>>.Failure(result.Errors, (int)HttpStatusCode.NotFound));

            return Ok(ResponseDto<IEnumerable<ContributionResponseDto>>.Success(result.Data, "Contributions retrieved successfully"));
        }

        /// <summary>
        /// Calculates benefit for a member.
        /// </summary>
        [HttpPost("benefits/calculate/{memberId}")]
        public async Task<IActionResult> CalculateBenefit(string memberId)
        {
            var result = await _benefitService.CalculateBenefit(memberId);
            if (!result.IsSuccess)
                return BadRequest(ResponseDto<BenefitResponseDto>.Failure(result.Errors));

            return Ok(ResponseDto<BenefitResponseDto>.Success(result.Data, "Benefit calculated successfully"));
        }

        /// <summary>
        /// Retrieves transaction history for a specific member.
        /// </summary>
        [HttpGet("transaction-history/member/{memberId}")]
        public async Task<IActionResult> GetTransactionHistoryByMemberId(string memberId, [FromQuery] PaginationRequestDto pagination)
        {
            var result = await _contributionService.GetTransactionHistoryByMemberId(memberId, pagination.pageSize, pagination.offset);

            if (!result.IsSuccess)
                return NotFound(ResponseDto<IEnumerable<TransactionHistoryDto>>.Failure(result.Errors, (int)HttpStatusCode.NotFound));

            return Ok(ResponseDto<IEnumerable<TransactionHistoryDto>>.Success(result.Data, "Transaction history retrieved successfully"));
        }

    }
}
