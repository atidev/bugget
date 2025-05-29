namespace Bugget.Entities.DbModels.Comment;

public sealed class CommentCreateDbModel
{
    public required int ReportId { get; init; }
    public required int BugId { get; init; }
    public required string Text { get; init; }
    public required string CreatorUserId { get; init; }
}