namespace Bugget.Entities.Views.Summary;

public sealed class ReportSummaryView
{
    public required int Id { get; init; }
    public required string Title { get; init; }
    public required int Status { get; init; }
    public required string ResponsibleUserId { get; init; }
    public required DateTimeOffset UpdatedAt { get; init; }
    public required string CreatorUserId { get; init; }
    public required DateTimeOffset CreatedAt { get; init; }
} 