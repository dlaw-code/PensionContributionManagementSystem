
namespace PensionContributionManagementSystem.Domain.Entities
{
    public class Benefit : Entity
    {
        public string MemberId { get; set; }
        public string BenefitType { get; set; }
        public DateTime CalculationDate { get; set; }
        public string EligibilityStatus { get; set; }
        public decimal Amount { get; set; }
    }


}
