namespace Bugget.Entities.Views.Users;

public sealed class UserView
{
    public required string Id { get; init; }
    public string? Name { get; init; }
    public string? PhotoUrl { get; init; }
}