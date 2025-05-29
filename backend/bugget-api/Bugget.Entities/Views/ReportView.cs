using Bugget.Entities.Views.Users;

namespace Bugget.Entities.Views;

public sealed class ReportView
{
    public required int Id { get; init; }
    public required string Title { get; init; }
    public required int Status { get; init; }
    public required UserView Responsible { get; init; }
    public required UserView Creator { get; init; }
    public required DateTimeOffset CreatedAt { get; init; }
    public required DateTimeOffset UpdatedAt { get; init; }
    public required UserView[] Participants { get; init; } 
    public BugView[]? Bugs { get; init; }
}