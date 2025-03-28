using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PensionContributionManagementSystem.Core.Dtos.Response
{
    public class BenefitResponseDto
    {
        public string Id { get; set; }
        public string MemberId { get; set; }
        public string BenefitType { get; set; }
        public DateTime CalculationDate { get; set; }
        public string EligibilityStatus { get; set; }
        public decimal Amount { get; set; }

    }
}
