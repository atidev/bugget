namespace Bugget.Entities.BO;

public sealed class Bug
{
    public int? Id { get; set; }
    public int? ReportId { get; set; }
    public required string Receive { get; set; }
    public required string Expect { get; set; }
    public DateTimeOffset? CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public required string CreatorUserId { get; set; }
    public int Status { get; set; } =  (int)BugStatus.Active;
    public Attachment[]? Attachments { get; set; }
    public Comment[]? Comments { get; set; }
}