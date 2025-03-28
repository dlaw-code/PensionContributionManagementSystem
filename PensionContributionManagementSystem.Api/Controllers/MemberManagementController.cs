using PensionContributionManagementSystem.Api.Dtos;
using PensionContributionManagementSystem.Core.Abstractions;
using PensionContributionManagementSystem.Core.Dtos;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Threading.Tasks;
using PensionContributionManagementSystem.Core.Dtos.Response;
using PensionContributionManagementSystem.Core.Dtos.Request;

namespace PensionContributionManagementSystem.API.Controllers
{
    [ApiController]
    [Route("memberManagement")]
    public class MemberManagementController : ControllerBase
    {
        private readonly IMemberManagementService _memberManagementService;

        public MemberManagementController(IMemberManagementService memberManagementService)
        {
            _memberManagementService = memberManagementService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserRequestDto registerUserDto)
        {
            var result = await _memberManagementService.Register(registerUserDto);

            if (result.IsFailure)
                return BadRequest(ResponseDto<RegisterResponseDto>.Failure(result.Errors));

            return Ok(ResponseDto<RegisterResponseDto>.Success(result.Data, "Registration successful"));
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUserDto loginUserDto)
        {
            var result = await _memberManagementService.Login(loginUserDto);

            if (result.IsFailure)
                return BadRequest(ResponseDto<LoginResponseDto>.Failure(result.Errors));

            return Ok(ResponseDto<LoginResponseDto>.Success(result.Data, "Login successful"));
        }

        [HttpGet("{memberId}")]
        public async Task<IActionResult> GetMemberById(string memberId)
        {
            var result = await _memberManagementService.GetMemberById(memberId);
            if (result.IsFailure)
                return NotFound(ResponseDto<MemberDto>.Failure(result.Errors, (int)HttpStatusCode.NotFound));

            return Ok(ResponseDto<MemberDto>.Success(result.Data, "Member retrieved successfully"));
        }

        [HttpPut("{memberId}")]
        public async Task<IActionResult> UpdateMember(string memberId, [FromBody] UpdateMemberRequestDto updateMemberDto)
        {
            var result = await _memberManagementService.UpdateMember(memberId, updateMemberDto);
            if (result.IsFailure)
                return BadRequest(ResponseDto<MemberDto>.Failure(result.Errors));

            return Ok(ResponseDto<MemberDto>.Success(result.Data, "Member updated successfully"));
        }

        [HttpDelete("{memberId}")]
        public async Task<IActionResult> DeleteMember(string memberId)
        {
            var result = await _memberManagementService.DeleteMember(memberId);
            if (result.IsFailure)
                return NotFound(ResponseDto<Result>.Failure(result.Errors, (int)HttpStatusCode.NotFound));

            return NoContent();
        }
    }
}
