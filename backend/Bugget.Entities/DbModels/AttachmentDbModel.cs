namespace Bugget.Entities.DbModels;

public sealed class AttachmentDbModel
{
    public int Id { get; init; }
    public int BugId { get; init; }
    public required string Path { get; init; }
    public DateTimeOffset? CreatedAt { get; init; }
    public required int AttachType { get; init; }
}