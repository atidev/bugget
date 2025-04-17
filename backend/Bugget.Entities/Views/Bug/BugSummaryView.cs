namespace Bugget.Entities.Views.Bug;

public sealed class BugSummaryView
{
    public required int Id { get; init; }
    public required int ReportId { get; init; }
    public required string Receive { get; init; }
    public required string Expect { get; init; }
    public required string CreatorUserId { get; init; }
    public required DateTimeOffset CreatedAt { get; init; }
    public required DateTimeOffset UpdatedAt { get; init; }
    public required int Status { get; init; }
} 