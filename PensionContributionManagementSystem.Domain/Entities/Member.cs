using Microsoft.AspNetCore.Identity;

namespace PensionContributionManagementSystem.Domain.Entities
{

    public class Member : IdentityUser, IAuditable
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTime DateOfBirth { get; set; }

        public bool IsDeleted { get; set; } = false;

        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }

        public string EmployerId { get; set; } = string.Empty;

        public  Employer Employer {  get; set; }
        public static Member Create(string firstname, string Lastname , string email, DateTime dateOfBirth , string emplyerId)
        {
            return new Member
            {
                Id = Guid.NewGuid().ToString(),
                FirstName = firstname,
                LastName = Lastname,
                DateOfBirth = dateOfBirth,
                Email = email,
                UserName = email,
                CreatedAt = DateTimeOffset.UtcNow,
                EmployerId = emplyerId
            };
        }

    }

    }


