using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PensionContributionManagementSystem.Core.Dtos.Response
{
    public class TransactionHistoryDto
    {
        public string Id { get; set; }
        public string EntityId { get; set; }
        public string EntityType { get; set; } // Member, Contribution, Benefit
        public string ChangeType { get; set; } // Created, Updated, Deleted
        public string ChangeDetails { get; set; }

        public string TimeStamp { get; set; }
    }
}
