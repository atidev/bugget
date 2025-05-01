namespace Bugget.Entities.Views.Users;

public sealed class UserView
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public required string? TeamId { get; init; }
    public required string? PhotoUrl { get; init; }
}