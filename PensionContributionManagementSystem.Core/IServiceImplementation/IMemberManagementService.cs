using PensionContributionManagementSystem.Core.Dtos;
using PensionContributionManagementSystem.Core.Dtos.Request;
using PensionContributionManagementSystem.Core.Dtos.Response;

namespace PensionContributionManagementSystem.Core.Abstractions;

public interface IMemberManagementService
{Task<Result<RegisterResponseDto>> Register(RegisterUserRequestDto registerUserDto);
    Task<Result<LoginResponseDto>> Login(LoginUserDto loginUserDto);
    Task<Result<MemberDto>> GetMemberById(string memberId);
    Task<Result<MemberDto>> UpdateMember(string memberId, UpdateMemberRequestDto updateMemberDto);
    Task<Result> DeleteMember(string memberId);
}