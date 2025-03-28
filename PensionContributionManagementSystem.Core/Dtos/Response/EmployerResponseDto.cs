using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PensionContributionManagementSystem.Core.Dtos.Response
{
    public class EmployerResponseDto
    {
        public string Id { get; set; }
        public string CompanyName { get; set; }
        public string RegistrationNumber { get; set; }
        public bool IsActive { get; set; }
    }
}
