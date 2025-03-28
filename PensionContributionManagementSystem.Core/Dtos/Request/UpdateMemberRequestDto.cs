using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PensionContributionManagementSystem.Core.Dtos.Request
{
    public class UpdateMemberRequestDto
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? EmployerId { get; set; }
        public DateTime? DateOfBirth { get; set; }
    }


}
