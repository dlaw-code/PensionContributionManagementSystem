using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PensionContributionManagementSystem.Core.Abstractions;
using PensionContributionManagementSystem.Domain.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class ReportService : IReportService
{
    private readonly IRepository<Contribution> _contributionRepository;
    private readonly UserManager<Member> _userManager;
    private readonly ILogger<ReportService> _logger;

    public ReportService(
        IRepository<Contribution> contributionRepository,
        UserManager<Member> userManager,
        ILogger<ReportService> logger)
    {
        _contributionRepository = contributionRepository;
        _userManager = userManager;
        _logger = logger;
    }

    public async Task GenerateContributionValidationReport()
    {
        _logger.LogInformation("Initiating contribution validation report generation");

        try
        {
            var contributions = await GetAllContributionsAsync();
            LogReportSummary(contributions.Count);
            OutputReportSummary(contributions.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate contribution validation report");
            throw;
        }
    }

    public async Task GenerateMemberStatements()
    {
        _logger.LogInformation("Starting member statement generation process");

        try
        {
            var members = await GetAllMembersAsync();
            await ProcessMemberStatements(members);

            _logger.LogInformation("Successfully generated statements for {MemberCount} members", members.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during member statement generation");
            throw;
        }
    }

    #region Private Helper Methods

    private async Task<List<Contribution>> GetAllContributionsAsync()
    {
        return await _contributionRepository.GetAll().ToListAsync();
    }

    private async Task<List<Member>> GetAllMembersAsync()
    {
        return await _userManager.Users.ToListAsync();
    }

    private async Task ProcessMemberStatements(List<Member> members)
    {
        foreach (var member in members)
        {
            await GenerateSingleMemberStatement(member);
        }
    }

    private async Task GenerateSingleMemberStatement(Member member)
    {
        _logger.LogDebug("Generating statement for member {MemberId}: {FirstName} {LastName}",
            member.Id, member.FirstName, member.LastName);

        Console.WriteLine($"Generated statement for {member.FirstName} {member.LastName}");
    }

    private void LogReportSummary(int contributionCount)
    {
        _logger.LogInformation("Validation report completed with {ContributionCount} contributions processed",
            contributionCount);
    }

    private void OutputReportSummary(int contributionCount)
    {
        Console.WriteLine($"Generated validation report with {contributionCount} contributions.");
    }

    #endregion
}