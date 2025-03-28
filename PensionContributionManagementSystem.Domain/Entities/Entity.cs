namespace PensionContributionManagementSystem.Domain.Entities;

public abstract class Entity : IAuditable
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}