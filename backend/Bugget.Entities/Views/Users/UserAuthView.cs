namespace Bugget.Entities.Views.Users;

public sealed class UserAuthView
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public required string? TeamId { get; set; }
}