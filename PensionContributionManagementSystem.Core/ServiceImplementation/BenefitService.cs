using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PensionContributionManagementSystem.Core.Abstractions;
using PensionContributionManagementSystem.Core.Dtos;
using PensionContributionManagementSystem.Core.Dtos.Response;
using PensionContributionManagementSystem.Domain.Entities;

public class BenefitService : IBenefitService
{
    private const decimal EligibilityThreshold = 100000m;
    private const decimal EligibleBenefitRate = 0.1m;
    private const string RetirementBenefitType = "Retirement";

    private readonly IRepository<TransactionHistory> _transactionRepository;
    private readonly IRepository<Benefit> _benefitRepository;
    private readonly IRepository<Contribution> _contributionRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<BenefitService> _logger;

    public BenefitService(
        IRepository<Benefit> benefitRepository,
        IRepository<Contribution> contributionRepository,
        IRepository<TransactionHistory> transactionRepository,
        IUnitOfWork unitOfWork,
        ILogger<BenefitService> logger)
    {
        _benefitRepository = benefitRepository;
        _contributionRepository = contributionRepository;
        _transactionRepository = transactionRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<BenefitResponseDto>> CalculateBenefit(string memberId)
    {
        _logger.LogInformation("Starting benefit calculation for member: {MemberId}", memberId);

        var contributions = await GetMemberContributions(memberId);
        if (!contributions.Any())
        {
            return HandleNoContributions(memberId);
        }

        var totalContributions = CalculateTotalContributions(contributions, memberId);
        var (eligibilityStatus, benefitAmount) = DetermineEligibilityAndAmount(totalContributions);

        var benefit = await CreateBenefitRecord(memberId, eligibilityStatus, benefitAmount);
        await RecordTransaction(memberId, "Created", "Benefit calculated.");

        await _unitOfWork.SaveChangesAsync();
        _logger.LogInformation("Benefit calculation saved for member: {MemberId}", memberId);

        return Result.Success(MapToResponseDto(benefit));
    }

    public async Task UpdateEligibilityStatus()
    {
        _logger.LogInformation("Updating eligibility status for all benefits.");

        var benefits = await _benefitRepository.GetAll().ToListAsync();

        foreach (var benefit in benefits)
        {
            UpdateSingleBenefitEligibility(benefit);
            await RecordTransaction(benefit.MemberId, "Updated", "Benefit eligibility updated.");
        }

        await _unitOfWork.SaveChangesAsync();
        _logger.LogInformation("Eligibility status update completed.");
    }

    private async Task<List<Contribution>> GetMemberContributions(string memberId)
    {
        return await _contributionRepository.GetAll()
            .Where(c => c.MemberId == memberId)
            .ToListAsync();
    }

    private Result<BenefitResponseDto> HandleNoContributions(string memberId)
    {
        _logger.LogWarning("No contributions found for member: {MemberId}", memberId);
        return Result.Failure<BenefitResponseDto>(new[] {
            new Error("CalculationFailed", "No contributions found for the member.")
        });
    }

    private decimal CalculateTotalContributions(List<Contribution> contributions, string memberId)
    {
        var total = contributions.Sum(c => c.Amount);
        _logger.LogInformation("Total contributions for member {MemberId}: {TotalContributions}",
            memberId, total);
        return total;
    }

    private (string EligibilityStatus, decimal BenefitAmount) DetermineEligibilityAndAmount(decimal totalContributions)
    {
        var isEligible = totalContributions >= EligibilityThreshold;
        return (
            isEligible ? "Eligible" : "Not Eligible",
            isEligible ? totalContributions * EligibleBenefitRate : 0
        );
    }

    private async Task<Benefit> CreateBenefitRecord(string memberId, string eligibilityStatus, decimal benefitAmount)
    {
        var benefit = new Benefit
        {
            MemberId = memberId,
            BenefitType = RetirementBenefitType,
            CalculationDate = DateTime.UtcNow,
            EligibilityStatus = eligibilityStatus,
            Amount = benefitAmount
        };

        await _benefitRepository.Add(benefit);
        _logger.LogInformation("Benefit record created for member {MemberId}: {BenefitAmount} ({Eligibility})",
            memberId, benefitAmount, eligibilityStatus);

        return benefit;
    }

    private async Task RecordTransaction(string entityId, string changeType, string changeDetails)
    {
        await _transactionRepository.Add(new TransactionHistory
        {
            EntityId = entityId,
            EntityType = nameof(Benefit),
            ChangeType = changeType,
            ChangeDetails = changeDetails
        });
    }

    private void UpdateSingleBenefitEligibility(Benefit benefit)
    {
        var oldStatus = benefit.EligibilityStatus;
        benefit.EligibilityStatus = benefit.Amount >= EligibilityThreshold ? "Eligible" : "Not Eligible";

        _logger.LogInformation("Updated eligibility for Member {MemberId}: {OldStatus} -> {NewStatus}",
            benefit.MemberId, oldStatus, benefit.EligibilityStatus);
    }

    private BenefitResponseDto MapToResponseDto(Benefit benefit)
    {
        return new BenefitResponseDto
        {
            Id = benefit.Id,
            MemberId = benefit.MemberId,
            BenefitType = benefit.BenefitType,
            Amount = benefit.Amount,
            EligibilityStatus = benefit.EligibilityStatus,
            CalculationDate = benefit.CalculationDate,
        };
    }
}