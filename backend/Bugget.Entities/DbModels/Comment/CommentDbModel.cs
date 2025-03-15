namespace Bugget.Entities.DbModels.Comment;

public sealed class CommentDbModel
{
    public required int Id { get; init; }
    public required int BugId { get; init; }
    public required string Text { get; init; }
    public required string CreatorUserId { get; init; }
    public required DateTimeOffset CreatedAt { get; init; }
    public required DateTimeOffset UpdatedAt { get; init; }
}