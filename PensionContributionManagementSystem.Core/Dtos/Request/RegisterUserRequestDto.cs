using System.ComponentModel.DataAnnotations;

namespace PensionContributionManagementSystem.Core.Dtos.Request;

public class RegisterUserRequestDto
{
    [Required] public string FirstName { get; set; }

    [Required] public string LastName { get; set; }
    [Required][EmailAddress] public string Email { get; set; }

    [Required] public DateTime DateOfBirth { get; set; }

    [Required] public string EmployerId { get; set; }
    [Required] public string Password { get; set; }
}