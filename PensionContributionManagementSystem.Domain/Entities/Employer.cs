

namespace PensionContributionManagementSystem.Domain.Entities
{

    public class Employer : Entity
    {
        public string CompanyName { get; set; }
        public string RegistrationNumber { get; set; }
        public bool IsActive { get; set; } = true;
    }

}
