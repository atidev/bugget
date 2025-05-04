using Bugget.Entities.DbModels.Attachment;
using Bugget.Entities.DbModels.Comment;

namespace Bugget.Entities.DbModels.Bug;

public sealed class BugCreateDbModel
{
    public int? Id { get; init; }
    public int? ReportId { get; init; }
    public required string Receive { get; init; }
    public required string Expect { get; init; }
    public DateTimeOffset? CreatedAt { get; init; }
    public DateTimeOffset? UpdatedAt { get; init; }
    public required string CreatorUserId { get; init; }
    public required int Status { get; init; }
    public AttachmentDbModel[]? Attachments { get; init; }
    public CommentDbModel[]? Comments { get; init; }
}