

namespace PensionContributionManagementSystem.Core.Abstractions
{
    public interface IReportService
    {
        Task GenerateContributionValidationReport();
        Task GenerateMemberStatements();
    }

}
