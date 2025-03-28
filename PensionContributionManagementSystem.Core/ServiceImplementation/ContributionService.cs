using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pension.CORE.Dtos.Request;
using PensionContributionManagementSystem.Core.Abstractions;
using PensionContributionManagementSystem.Core.Dtos;
using PensionContributionManagementSystem.Core.Dtos.Response;
using PensionContributionManagementSystem.Domain.Constants;
using PensionContributionManagementSystem.Domain.Entities;

namespace PensionContributionManagementSystem.Core.Services
{
    public class ContributionService : IContributionService
    {
        private const decimal MonthlyInterestRate = 0.05m;
        private readonly IRepository<Contribution> _contributionRepository;
        private readonly IRepository<TransactionHistory> _transactionRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ContributionService> _logger;

        public ContributionService(
            IRepository<Contribution> contributionRepository,
            IRepository<TransactionHistory> transactionRepository,
            IUnitOfWork unitOfWork,
            ILogger<ContributionService> logger)
        {
            _contributionRepository = contributionRepository;
            _transactionRepository = transactionRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Result<ContributionResponseDto>> AddContribution(ContributionRequestDto contributionDto)
        {
            _logger.LogInformation("Starting contribution addition for Member ID: {MemberId}", contributionDto.MemberId);

            var validationResult = await ValidateContribution(contributionDto);
            if (!validationResult.IsSuccess)
            {
                return validationResult;
            }

            var contribution = CreateContributionEntity(contributionDto);
            await _contributionRepository.Add(contribution);

            await RecordTransaction(
                contribution.MemberId,
                "Created",
                $"New {contribution.ContributionType} contribution added.");

            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("Contribution successfully saved for Member ID: {MemberId}", contributionDto.MemberId);

            return Result.Success(MapToContributionResponseDto(contribution));
        }

        public async Task<Result<IEnumerable<ContributionResponseDto>>> GetMemberContributions(string memberId, int pageSize, int offset)
        {
            _logger.LogInformation("Fetching contributions for Member ID: {MemberId}", memberId);

            var contributions = await GetPagedContributions(memberId, pageSize, offset);

            if (!contributions.Any())
            {
                _logger.LogWarning("No contributions found for Member ID: {MemberId}", memberId);
            }
            else
            {
                _logger.LogInformation("{Count} contributions found for Member ID: {MemberId}", contributions.Count, memberId);
            }

            return Result.Success(contributions.AsEnumerable());
        }

        public async Task<Result<IEnumerable<TransactionHistoryDto>>> GetTransactionHistoryByMemberId(string memberId, int pageSize, int offset)
        {
            _logger.LogInformation("Fetching transaction history for Member ID: {MemberId}", memberId);

            var transactions = await GetPagedTransactions(memberId, pageSize, offset);

            if (!transactions.Any())
            {
                _logger.LogWarning("No transaction history found for Member ID: {MemberId}", memberId);
                return Result.Failure<IEnumerable<TransactionHistoryDto>>(
                    new[] { new Error("NotFound", "No transaction history found.") });
            }

            _logger.LogInformation("{Count} transactions found for Member ID: {MemberId}", transactions.Count, memberId);
            return Result.Success(transactions.AsEnumerable());
        }

        public async Task CalculateMonthlyInterest()
        {
            _logger.LogInformation("Starting monthly interest calculation for all contributions.");

            var contributions = await _contributionRepository.GetAll().ToListAsync();

            foreach (var contribution in contributions)
            {
                await ProcessContributionInterest(contribution);
            }

            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("Monthly interest calculation completed.");
        }

        #region Private Helper Methods

        private async Task<Result<ContributionResponseDto>> ValidateContribution(ContributionRequestDto contributionDto)
        {
            if (contributionDto.ContributionType == ContributionEnum.Monthly.ToString())
            {
                var exists = await _contributionRepository.GetAll()
                    .Where(c => c.MemberId == contributionDto.MemberId &&
                               c.ContributionType == ContributionEnum.Monthly.ToString())
                    .AnyAsync(c => c.ContributionDate.Month == contributionDto.ContributionDate.Month &&
                                 c.ContributionDate.Year == contributionDto.ContributionDate.Year);

                if (exists)
                {
                    _logger.LogWarning("Monthly contribution exists for Member ID: {MemberId} in {Month}/{Year}",
                        contributionDto.MemberId, contributionDto.ContributionDate.Month, contributionDto.ContributionDate.Year);

                    return Result.Failure<ContributionResponseDto>(
                        new[] { new Error("ValidationFailed", "Monthly contribution already exists for this month.") });
                }
            }
            return Result.Success<ContributionResponseDto>(null);
        }

        private Contribution CreateContributionEntity(ContributionRequestDto dto)
        {
            _logger.LogInformation("Creating contribution for Member ID: {MemberId}, Amount: {Amount}, Type: {Type}",
                dto.MemberId, dto.Amount, dto.ContributionType);

            return new Contribution
            {
                MemberId = dto.MemberId,
                ContributionType = dto.ContributionType,
                Amount = dto.Amount,
                ContributionDate = dto.ContributionDate,
                ReferenceNumber = dto.ReferenceNumber
            };
        }

        private async Task<List<ContributionResponseDto>> GetPagedContributions(string memberId, int pageSize, int offset)
        {
            return await _contributionRepository.GetAll()
                .Where(c => c.MemberId == memberId)
                .OrderByDescending(c => c.ContributionDate)
                .Select(c => MapToContributionResponseDto(c))
                .Skip(offset)
                .Take(pageSize)
                .ToListAsync();
        }

        private async Task<List<TransactionHistoryDto>> GetPagedTransactions(string memberId, int pageSize, int offset)
        {
            return await _transactionRepository.GetAll()
                .Where(t => t.EntityId == memberId)
                .OrderByDescending(t => t.CreatedAt)
                .Select(t => new TransactionHistoryDto
                {
                    Id = t.Id,
                    EntityId = t.EntityId,
                    EntityType = t.EntityType,
                    ChangeType = t.ChangeType,
                    ChangeDetails = t.ChangeDetails,
                    TimeStamp = t.CreatedAt.ToString(),
                })
                .Skip(offset)
                .Take(pageSize)
                .ToListAsync();
        }

        private async Task ProcessContributionInterest(Contribution contribution)
        {
            decimal interest = contribution.Amount * MonthlyInterestRate;

            _logger.LogInformation("Calculated interest for Member ID: {MemberId}, Contribution ID: {ContributionId}, Interest: {InterestAmount}",
                contribution.MemberId, contribution.Id, interest);

            await RecordTransaction(
                contribution.MemberId,
                "Updated",
                $"Interest calculated: {interest:C}");
        }

        private async Task RecordTransaction(string entityId, string changeType, string changeDetails)
        {
            await _transactionRepository.Add(new TransactionHistory
            {
                EntityId = entityId,
                EntityType = nameof(Contribution),
                ChangeType = changeType,
                ChangeDetails = changeDetails
            });
        }

        private ContributionResponseDto MapToContributionResponseDto(Contribution contribution)
        {
            return new ContributionResponseDto
            {
                Id = contribution.Id,
                MemberId = contribution.MemberId,
                ContributionType = contribution.ContributionType,
                Amount = contribution.Amount,
                ContributionDate = contribution.ContributionDate,
                ReferenceNumber = contribution.ReferenceNumber
            };
        }

        #endregion
    }
}