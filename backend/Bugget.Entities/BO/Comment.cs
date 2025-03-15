namespace Bugget.Entities.BO;

public sealed class Comment
{
    public int? Id { get; set; }
    public required int BugId { get; set; }
    public required int ReportId { get; set; }
    public required string Text { get; set; }
    public required string CreatorUserId { get; set; }
    public DateTimeOffset? CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}