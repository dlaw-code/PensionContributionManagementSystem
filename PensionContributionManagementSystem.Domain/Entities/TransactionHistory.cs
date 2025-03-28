

namespace PensionContributionManagementSystem.Domain.Entities
{

    public class TransactionHistory : Entity
    {
        public string EntityId { get; set; }
        public string EntityType { get; set; } // Member, Contribution, Benefit
        public string ChangeType { get; set; } // Created, Updated, Deleted
        public string ChangeDetails { get; set; }
    }

}
