using PensionContributionManagementSystem.Domain.Entities;

namespace PensionContributionManagementSystem.Core.Abstractions;

public interface IJwtService
{
    public string GenerateToken(Member user);
}