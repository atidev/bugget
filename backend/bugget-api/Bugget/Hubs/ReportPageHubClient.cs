using Bugget.DA.WebSockets;
using Bugget.Entities.BO;
using Bugget.Entities.DbModels.Bug;
using Bugget.Entities.DbModels.Comment;
using Bugget.Entities.DTO.Bug;
using Bugget.Entities.SocketViews;
using Microsoft.AspNetCore.SignalR;

namespace Bugget.Hubs;

public class ReportPageHubClient(IHubContext<ReportPageHub> hubContext) : IReportPageHubClient
{
    public Task SendReportPatchAsync(int reportId, PatchReportSocketView view, string? signalRConnectionId)
    {
        if (signalRConnectionId == null)
        {
            return hubContext.Clients.Group($"{reportId}")
                .SendAsync("ReceiveReportPatch", view);
        }

        return hubContext.Clients.GroupExcept($"{reportId}", signalRConnectionId)
            .SendAsync("ReceiveReportPatch", view);
    }

    public Task SendNewReportParticipantAsync(int reportId, string newParticipant)
    {
        return hubContext.Clients.Group($"{reportId}")
            .SendAsync("ReceiveReportParticipant", newParticipant);
    }

    public Task SendBugPatchAsync(int reportId, int bugId, BugPatchDto patchDto, string? signalRConnectionId)
    {
        if (signalRConnectionId == null)
        {
            return hubContext.Clients.Group($"{reportId}")
                .SendAsync("ReceiveBugPatch", bugId, patchDto);
        }

        return hubContext.Clients.GroupExcept($"{reportId}", signalRConnectionId)
            .SendAsync("ReceiveBugPatch", bugId, patchDto);
    }

    public Task SendBugCreateAsync(int reportId, BugSummaryDbModel summaryDbModel, string? signalRConnectionId)
    {
        if (signalRConnectionId == null)
        {
            return hubContext.Clients.Group($"{reportId}")
                .SendAsync("ReceiveBugCreate", summaryDbModel);
        }

        return hubContext.Clients.GroupExcept($"{reportId}", signalRConnectionId)
            .SendAsync("ReceiveBugCreate", summaryDbModel);
    }

    public Task SendAttachmentCreateAsync(int reportId, AttachmentSocketView attachmentSocketView, string? signalRConnectionId)
    {
        string eventName = attachmentSocketView.AttachType == (int)AttachType.Comment
        ? "ReceiveCommentAttachmentCreate"
        : "ReceiveBugAttachmentCreate";

        if (signalRConnectionId == null)
        {
            return hubContext.Clients.Group($"{reportId}")
                .SendAsync(eventName, attachmentSocketView);
        }

        return hubContext.Clients.GroupExcept($"{reportId}", signalRConnectionId)
            .SendAsync(eventName, attachmentSocketView);
    }

    public Task SendAttachmentOptimizedAsync(int reportId, AttachmentSocketView attachmentSocketView)
    {
        string eventName = attachmentSocketView.AttachType == (int)AttachType.Comment
        ? "ReceiveCommentAttachmentOptimized"
        : "ReceiveBugAttachmentOptimized";

        return hubContext.Clients.Group($"{reportId}")
            .SendAsync(eventName, attachmentSocketView);
    }

    public Task SendAttachmentDeleteAsync(int reportId, int id ,int entityId, int attachType, string? signalRConnectionId)
    {
        string eventName = attachType == (int)AttachType.Comment
        ? "ReceiveCommentAttachmentDelete"
        : "ReceiveBugAttachmentDelete";

        if (signalRConnectionId == null)
        {
            return hubContext.Clients.Group($"{reportId}")
                .SendAsync(eventName, id, entityId, attachType);
        }

        return hubContext.Clients.GroupExcept($"{reportId}", signalRConnectionId)
            .SendAsync(eventName, id, entityId, attachType);
    }

    public Task SendCommentCreateAsync(int reportId, CommentSummaryDbModel commentSummaryDbModel, string? signalRConnectionId)
    {
        if (signalRConnectionId == null)
        {
            return hubContext.Clients.Group($"{reportId}")
                .SendAsync("ReceiveCommentCreate", commentSummaryDbModel);
        }

        return hubContext.Clients.GroupExcept($"{reportId}", signalRConnectionId)
            .SendAsync("ReceiveCommentCreate", commentSummaryDbModel);
    }

    public Task SendCommentDeleteAsync(int reportId, int bugId, int commentId, string? signalRConnectionId)
    {
        if (signalRConnectionId == null)
        {
            return hubContext.Clients.Group($"{reportId}")
                .SendAsync("ReceiveCommentDelete", bugId, commentId);
        }

        return hubContext.Clients.GroupExcept($"{reportId}", signalRConnectionId)
            .SendAsync("ReceiveCommentDelete", bugId, commentId);
    }

    public Task SendCommentUpdateAsync(int reportId, CommentSummaryDbModel commentSummaryDbModel, string? signalRConnectionId)
    {
        if (signalRConnectionId == null)
        {
            return hubContext.Clients.Group($"{reportId}")
                .SendAsync("ReceiveCommentUpdate", commentSummaryDbModel);
        }

        return hubContext.Clients.GroupExcept($"{reportId}", signalRConnectionId)
            .SendAsync("ReceiveCommentUpdate", commentSummaryDbModel);
    }
}