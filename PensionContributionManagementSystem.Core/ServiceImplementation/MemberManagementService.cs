using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using PensionContributionManagementSystem.Core.Abstractions;
using PensionContributionManagementSystem.Core.Dtos;
using PensionContributionManagementSystem.Core.Dtos.Request;
using PensionContributionManagementSystem.Core.Dtos.Response;
using PensionContributionManagementSystem.Domain.Entities;
using System.Linq;
using System.Threading.Tasks;

namespace PensionContributionManagementSystem.Core.Services
{
    public class MemberManagementService : IMemberManagementService
    {
        private readonly IJwtService _jwtService;
        private readonly UserManager<Member> _userManager;
        private readonly ILogger<MemberManagementService> _logger;

        public MemberManagementService(
            UserManager<Member> userManager,
            IJwtService jwtService,
            ILogger<MemberManagementService> logger)
        {
            _userManager = userManager;
            _jwtService = jwtService;
            _logger = logger;
        }

        public async Task<Result<RegisterResponseDto>> Register(RegisterUserRequestDto registerUserDto)
        {
            _logger.LogInformation("Starting registration for user with email: {Email}", registerUserDto.Email);

            var user = Member.Create(
                registerUserDto.FirstName,
                registerUserDto.LastName,
                registerUserDto.Email,
                registerUserDto.DateOfBirth,
                registerUserDto.EmployerId);

            var creationResult = await _userManager.CreateAsync(user, registerUserDto.Password);
            if (!creationResult.Succeeded)
            {
                return HandleFailedRegistration(registerUserDto.Email, creationResult);
            }

            var newUser = await FindUserByEmail(registerUserDto.Email);
            if (newUser == null)
            {
                return HandleUserNotFoundAfterCreation(registerUserDto.Email);
            }

            _logger.LogInformation("User registered successfully with ID: {UserId}", newUser.Id);
            return Result.Success(MapToRegisterResponseDto(newUser));
        }

        public async Task<Result<LoginResponseDto>> Login(LoginUserDto loginUserDto)
        {
            _logger.LogInformation("Attempting login for user with email: {Email}", loginUserDto.Email);

            var user = await FindUserByEmail(loginUserDto.Email);
            if (user == null || !await ValidateUserPassword(user, loginUserDto.Password))
            {
                return HandleFailedLogin(loginUserDto.Email);
            }

            var token = _jwtService.GenerateToken(user);
            _logger.LogInformation("User logged in successfully with email: {Email}", loginUserDto.Email);

            return new LoginResponseDto(token);
        }

        public async Task<Result<MemberDto>> GetMemberById(string memberId)
        {
            _logger.LogInformation("Fetching member details for ID: {MemberId}", memberId);

            var user = await FindActiveUserById(memberId);
            if (user == null)
            {
                return HandleMemberNotFound(memberId);
            }

            _logger.LogInformation("Member found with ID: {MemberId}", memberId);
            return Result.Success(MapToMemberDto(user));
        }

        public async Task<Result<MemberDto>> UpdateMember(string memberId, UpdateMemberRequestDto updateMemberDto)
        {
            _logger.LogInformation("Updating member details for ID: {MemberId}", memberId);

            var user = await FindActiveUserById(memberId);
            if (user == null)
            {
                return HandleMemberNotFound(memberId);
            }

            UpdateUserProperties(user, updateMemberDto);
            var updateResult = await _userManager.UpdateAsync(user);

            if (!updateResult.Succeeded)
            {
                return HandleFailedUpdate(memberId, updateResult);
            }

            _logger.LogInformation("Member updated successfully. ID: {MemberId}", memberId);
            return Result.Success(MapToMemberDto(user));
        }

        public async Task<Result> DeleteMember(string memberId)
        {
            _logger.LogInformation("Deleting member with ID: {MemberId}", memberId);

            var user = await FindActiveUserById(memberId);
            if (user == null)
            {
                return HandleMemberNotFound(memberId);
            }

            return await SoftDeleteUser(user, memberId);
        }

        #region Private Helper Methods

        private async Task<Member> FindUserByEmail(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        private async Task<Member> FindActiveUserById(string memberId)
        {
            var user = await _userManager.FindByIdAsync(memberId);
            return user is not null && !user.IsDeleted ? user : null;
        }

        private async Task<bool> ValidateUserPassword(Member user, string password)
        {
            return await _userManager.CheckPasswordAsync(user, password);
        }

        private Result<RegisterResponseDto> HandleFailedRegistration(string email, IdentityResult result)
        {
            _logger.LogWarning("User registration failed for email: {Email}. Errors: {Errors}",
                email, result.Errors.Select(e => e.Description));

            return Result.Failure<RegisterResponseDto>(
                result.Errors.Select(error => new Error(error.Code, error.Description)).ToArray());
        }

        private Result<RegisterResponseDto> HandleUserNotFoundAfterCreation(string email)
        {
            _logger.LogError("Failed to find newly registered user with email: {Email}", email);
            return Result.Failure<RegisterResponseDto>(
                new[] { new Error("UserError", "Failed to find user after creation.") });
        }

        private Result<LoginResponseDto> HandleFailedLogin(string email)
        {
            _logger.LogWarning("Login failed for email: {Email}. Invalid credentials.", email);
            return new Error[] { new("MemberManagement.Error", "Email or password not correct") };
        }

        private Result<MemberDto> HandleMemberNotFound(string memberId)
        {
            _logger.LogWarning("Member not found or deleted. ID: {MemberId}", memberId);
            return Result.Failure<MemberDto>(
                new[] { new Error("MemberManagement.NotFound", "Member not found or deleted") });
        }

        private Result<MemberDto> HandleFailedUpdate(string memberId, IdentityResult result)
        {
            _logger.LogError("Failed to update member with ID: {MemberId}. Errors: {Errors}",
                memberId, result.Errors.Select(e => e.Description));

            return Result.Failure<MemberDto>(
                result.Errors.Select(e => new Error(e.Code, e.Description)).ToArray());
        }

        private async Task<Result> SoftDeleteUser(Member user, string memberId)
        {
            user.IsDeleted = true;
            user.UpdatedAt = DateTimeOffset.UtcNow;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                _logger.LogError("Failed to delete member with ID: {MemberId}. Errors: {Errors}",
                    memberId, result.Errors.Select(e => e.Description));

                return Result.Failure(
                    result.Errors.Select(e => new Error(e.Code, e.Description)).ToArray());
            }

            _logger.LogInformation("Member successfully marked as deleted. ID: {MemberId}", memberId);
            return Result.Success();
        }

        private void UpdateUserProperties(Member user, UpdateMemberRequestDto updateDto)
        {
            user.FirstName = updateDto.FirstName ?? user.FirstName;
            user.LastName = updateDto.LastName ?? user.LastName;
            user.Email = updateDto.Email ?? user.Email;
            user.UserName = updateDto.Email ?? user.Email;
            user.DateOfBirth = updateDto.DateOfBirth ?? user.DateOfBirth;
            user.EmployerId = updateDto.EmployerId ?? user.EmployerId;
            user.UpdatedAt = DateTimeOffset.UtcNow;
        }

        private RegisterResponseDto MapToRegisterResponseDto(Member user)
        {
            return new RegisterResponseDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email!,
                DateOfBirth = user.DateOfBirth,
                CreatedAt = user.CreatedAt
            };
        }

        private MemberDto MapToMemberDto(Member user)
        {
            return new MemberDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                DateOfBirth = user.DateOfBirth,
                CreatedAt = user.CreatedAt
            };
        }

        #endregion
    }
}