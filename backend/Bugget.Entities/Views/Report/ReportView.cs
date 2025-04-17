namespace Bugget.Entities.Views;

public sealed class ReportView
{
    public required int Id { get; init; }
    public required string Title { get; init; }
    public required int Status { get; init; }
    public required string ResponsibleUserId { get; init; }
    public required string[] ParticipantsUserIds { get; init; }
    public required DateTimeOffset UpdatedAt { get; init; }
    public required string CreatorUserId { get; init; }
    public string? CreatorTeamId { get; init; }
    public string? CreatorOrganizationId { get; init; }
    public required DateTimeOffset CreatedAt { get; init; }
    public required BugView[] Bugs { get; init; }
}