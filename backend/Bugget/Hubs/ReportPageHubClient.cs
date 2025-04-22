using Bugget.DA.WebSockets;
using Bugget.Entities.DbModels.Bug;
using Bugget.Entities.DTO.Bug;
using Bugget.Entities.SocketViews;
using Microsoft.AspNetCore.SignalR;

namespace Bugget.Hubs;

public class ReportPageHubClient(IHubContext<ReportPageHub> hubContext) : IReportPageHubClient
{
    private readonly IHubContext<ReportPageHub> _hubContext = hubContext;

    public Task SendReportPatchAsync(int reportId, PatchReportSocketView view, string? signalRConnectionId)
    {
        if (signalRConnectionId == null)
        {
            return _hubContext.Clients.Group($"{reportId}")
                .SendAsync("ReceiveReportPatch", view);
        }

        return _hubContext.Clients.GroupExcept($"{reportId}", signalRConnectionId)
            .SendAsync("ReceiveReportPatch", view);
    }

    public Task SendReportParticipantsAsync(int reportId, string[] participants)
    {
        return _hubContext.Clients.Group($"{reportId}")
            .SendAsync("ReceiveReportParticipants", participants);
    }

    public Task SendBugPatchAsync(int reportId, int bugId, BugPatchDto patchDto, string? signalRConnectionId)
    {
        if (signalRConnectionId == null)
        {
            return _hubContext.Clients.Group($"{reportId}")
                .SendAsync("ReceiveBugPatch", bugId, patchDto);
        }

        return _hubContext.Clients.GroupExcept($"{reportId}", signalRConnectionId)
            .SendAsync("ReceiveBugPatch", bugId, patchDto);
    }

    public Task SendBugCreateAsync(int reportId, BugSummaryDbModel summaryDbModel, string? signalRConnectionId)
    {
        if (signalRConnectionId == null)
        {
            return _hubContext.Clients.Group($"{reportId}")
                .SendAsync("ReceiveBugCreate", summaryDbModel);
        }

        return _hubContext.Clients.GroupExcept($"{reportId}", signalRConnectionId)
            .SendAsync("ReceiveBugCreate", summaryDbModel);
    }
} 