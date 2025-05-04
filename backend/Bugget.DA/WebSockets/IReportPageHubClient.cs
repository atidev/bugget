using Bugget.Entities.DbModels.Bug;
using Bugget.Entities.DbModels.Comment;
using Bugget.Entities.DTO.Bug;
using Bugget.Entities.SocketViews;

namespace Bugget.DA.WebSockets;

public interface IReportPageHubClient
{
    Task SendReportPatchAsync(int reportId, PatchReportSocketView view, string? signalRConnectionId);
    Task SendNewReportParticipantAsync(int reportId, string newParticipant);
    Task SendBugCreateAsync(int reportId, BugSummaryDbModel summaryDbModel, string? signalRConnectionId);
    Task SendBugPatchAsync(int reportId, int bugId, BugPatchDto patchDto, string? signalRConnectionId);
    Task SendAttachmentCreateAsync(int reportId, AttachmentSocketView attachmentSocketView, string? signalRConnectionId);
    Task SendAttachmentDeleteAsync(int reportId, AttachmentSocketView attachmentSocketView, string? signalRConnectionId);
    Task SendAttachmentOptimizedAsync(int reportId, AttachmentSocketView attachmentSocketView);
    Task SendCommentCreateAsync(int reportId, CommentSummaryDbModel commentSummaryDbModel, string? signalRConnectionId);
    Task SendCommentDeleteAsync(int reportId, int bugId, int commentId, string? signalRConnectionId);
    Task SendCommentUpdateAsync(int reportId, CommentSummaryDbModel commentSummaryDbModel, string? signalRConnectionId);
}