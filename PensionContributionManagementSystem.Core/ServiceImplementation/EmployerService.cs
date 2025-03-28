using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PensionContributionManagementSystem.Core.Abstractions;
using PensionContributionManagementSystem.Core.Dtos;
using PensionContributionManagementSystem.Core.Dtos.Request;
using PensionContributionManagementSystem.Core.Dtos.Response;
using PensionContributionManagementSystem.Domain.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PensionContributionManagementSystem.Core.Services
{
    public class EmployerService : IEmployerService
    {
        private readonly IRepository<Employer> _employerRepository;
        private readonly UserManager<Member> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<EmployerService> _logger;

        public EmployerService(
            IRepository<Employer> employerRepository,
            UserManager<Member> userManager,
            IUnitOfWork unitOfWork,
            ILogger<EmployerService> logger)
        {
            _employerRepository = employerRepository;
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Result<EmployerResponseDto>> AddEmployer(AddEmployerDto employerDto)
        {
            _logger.LogInformation("Adding new employer: {CompanyName}", employerDto.CompanyName);

            var validationResult = await ValidateEmployerRegistration(employerDto.RegistrationNumber);
            if (!validationResult.IsSuccess)
            {
                return validationResult;
            }

            var employer = CreateEmployerEntity(employerDto);
            await _employerRepository.Add(employer);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Employer {CompanyName} added successfully with Registration Number: {RegistrationNumber}",
                employer.CompanyName, employer.RegistrationNumber);

            return Result.Success(MapToEmployerResponseDto(employer));
        }

        public async Task<Result<EmployerDto>> GetEmployerWithMembers(string employerId)
        {
            _logger.LogInformation("Fetching employer with members for ID: {EmployerId}", employerId);

            var employer = await _employerRepository.FindById(employerId);
            if (employer == null)
            {
                return HandleEmployerNotFound(employerId);
            }

            var members = await GetEmployerMembers(employerId);
            _logger.LogInformation("Found {MemberCount} members for Employer ID: {EmployerId}", members.Count, employerId);

            return Result.Success(MapToEmployerDto(employer, members));
        }

        #region Private Helper Methods

        private async Task<Result<EmployerResponseDto>> ValidateEmployerRegistration(string registrationNumber)
        {
            var existingEmployer = await _employerRepository.GetAll()
                .FirstOrDefaultAsync(e => e.RegistrationNumber == registrationNumber);

            if (existingEmployer != null)
            {
                _logger.LogWarning("Employer with registration number {RegistrationNumber} already exists", registrationNumber);
                return Result.Failure<EmployerResponseDto>(
                    new[] { new Error("Duplicate", $"Employer with registration number {registrationNumber} already exists.") });
            }

            return Result.Success<EmployerResponseDto>(null);
        }

        private Employer CreateEmployerEntity(AddEmployerDto dto)
        {
            return new Employer
            {
                CompanyName = dto.CompanyName,
                RegistrationNumber = dto.RegistrationNumber,
                IsActive = true
            };
        }

        private async Task<List<Member>> GetEmployerMembers(string employerId)
        {
            return await _userManager.Users
                .Where(m => m.EmployerId == employerId)
                .ToListAsync();
        }

        private Result<EmployerDto> HandleEmployerNotFound(string employerId)
        {
            _logger.LogWarning("Employer with ID {EmployerId} not found", employerId);
            return Result.Failure<EmployerDto>(
                new[] { new Error("NotFound", "Employer not found.") });
        }

        private EmployerResponseDto MapToEmployerResponseDto(Employer employer)
        {
            return new EmployerResponseDto
            {
                Id = employer.Id,
                CompanyName = employer.CompanyName,
                RegistrationNumber = employer.RegistrationNumber,
                IsActive = employer.IsActive
            };
        }

        private EmployerDto MapToEmployerDto(Employer employer, List<Member> members)
        {
            return new EmployerDto
            {
                Id = employer.Id,
                CompanyName = employer.CompanyName,
                RegistrationNumber = employer.RegistrationNumber,
                IsActive = employer.IsActive,
                Members = members.Select(m => new MemberDto
                {
                    Id = m.Id,
                    FirstName = m.FirstName,
                    LastName = m.LastName,
                    Email = m.Email,
                    DateOfBirth = m.DateOfBirth,
                    CreatedAt = m.CreatedAt
                }).ToList()
            };
        }

        #endregion
    }
}