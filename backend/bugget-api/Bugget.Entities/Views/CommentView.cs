using Bugget.Entities.Views.Attachment;
using Bugget.Entities.Views.Users;

namespace Bugget.Entities.Views;

public sealed class CommentView
{
    public required int Id { get; init; }
    public required int BugId { get; init; }
    public required string Text { get; init; }
    public required UserView Creator { get; init; }
    public required DateTimeOffset CreatedAt { get; init; }
    public required DateTimeOffset UpdatedAt { get; init; }
    public AttachmentView[]? Attachments { get; init; }
}