using PensionContributionManagementSystem.Core.Dtos;
using PensionContributionManagementSystem.Core.Dtos.Request;
using PensionContributionManagementSystem.Core.Dtos.Response;
using PensionContributionManagementSystem.Domain.Entities;

namespace PensionContributionManagementSystem.Core.Abstractions
{
    public interface IEmployerService
    {
        Task<Result<EmployerResponseDto>> AddEmployer(AddEmployerDto employer);
        Task<Result<EmployerDto>> GetEmployerWithMembers(string employerId);
    }
}
