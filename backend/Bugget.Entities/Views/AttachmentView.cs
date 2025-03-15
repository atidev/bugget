namespace Bugget.Entities.Views;

public sealed class AttachmentView
{
    public required int Id { get; init; }
    public required int BugId { get; init; }
    public required int ReportId { get; init; }
    public required string Path { get; init; }
    public required DateTimeOffset CreatedAt { get; init; }
    public required int AttachType { get; init; }
}