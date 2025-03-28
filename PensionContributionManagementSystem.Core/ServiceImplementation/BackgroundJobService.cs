using Hangfire;
using Microsoft.Extensions.Logging;
using PensionContributionManagementSystem.Core.Abstractions;

namespace PensionContributionManagementSystem.Core.Services;

public class BackgroundJobService
{
    private readonly ILogger<BackgroundJobService> _logger;
    private readonly IContributionService _contributionService;
    private readonly IBenefitService _benefitService;
    private readonly IReportService _reportService;

    public BackgroundJobService(
        ILogger<BackgroundJobService> logger,
        IContributionService contributionService,
        IBenefitService benefitService,
        IReportService reportService)
    {
        _logger = logger;
        _contributionService = contributionService;
        _benefitService = benefitService;
        _reportService = reportService;
    }

    public void ScheduleJobs()
    {
        _logger.LogInformation("Scheduling background jobs...");

        // This method runs validation report on the 1st of every month at midnight
        RecurringJob.AddOrUpdate("MonthlyContributionValidation",
            () => _reportService.GenerateContributionValidationReport(), Cron.Monthly);

        // This method benefit eligibility update on the 5th of every month
        RecurringJob.AddOrUpdate("BenefitEligibilityUpdate",
            () => _benefitService.UpdateEligibilityStatus(), "0 0 5 * *");

        // This method runs simple interest calculations on the 10th of every month
        RecurringJob.AddOrUpdate("MonthlyInterestCalculation",
            () => _contributionService.CalculateMonthlyInterest(), "0 0 10 * *");

        // This method generate member statements on the 15th of every month
        RecurringJob.AddOrUpdate("GenerateMemberStatements",
            () => _reportService.GenerateMemberStatements(), "0 0 15 * *");

        _logger.LogInformation("Background jobs scheduled.");
    }
}
