

namespace PensionContributionManagementSystem.Domain.Entities
{

    public class Contribution : Entity
    {
        public string MemberId { get; set; } 
        public string ContributionType { get; set; } 
        public decimal Amount { get; set; }
        public DateTime ContributionDate { get; set; } 
        public string ReferenceNumber { get; set; }
    }

}
