using System.ComponentModel.DataAnnotations;

namespace PensionContributionManagementSystem.Core.Dtos.Request;

public class LoginUserDto
{
    [Required] public string Email { get; set; }

    [Required] public string Password { get; set; }
}