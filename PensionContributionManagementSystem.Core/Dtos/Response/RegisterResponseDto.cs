namespace PensionContributionManagementSystem.Core.Dtos.Response
{
    public class RegisterResponseDto
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public DateTime DateOfBirth { get; set; }
        public DateTimeOffset CreatedAt { get; set; }

    }
}
