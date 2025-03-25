using Bugget.Entities.Views.Users;

namespace Bugget.Entities.Views;

public sealed class BugView
{
    public required int Id { get; init; }
    public required int ReportId { get; init; }
    public required string Receive { get; init; }
    public required string Expect { get; init; }
    public required UserView Creator { get; init; }
    public required DateTimeOffset CreatedAt { get; init; }
    public required DateTimeOffset UpdatedAt { get; init; }
    public required int Status { get; init; }
    public AttachmentView[]? Attachments { get; init; }
    public CommentView[]? Comments { get; init; }
}