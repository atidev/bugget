namespace Bugget.Entities.BO;

public sealed class Attachment
{
    public int? Id { get; set; }
    public int BugId { get; set; }
    public required string Path { get; set; }
    public DateTimeOffset? CreatedAt { get; set; }
    public int AttachType { get; set; }
}