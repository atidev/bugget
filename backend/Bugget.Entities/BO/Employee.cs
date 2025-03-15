namespace Bugget.Entities.BO;

public class Employee
{
    public required string Id { get; init; }
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public string? Surname { get; init; }
    public string? NotificationUserId { get; init; }
}