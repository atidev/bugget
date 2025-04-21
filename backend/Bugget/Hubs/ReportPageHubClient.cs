using Bugget.BO.WebSockets;
using Bugget.Entities.SocketViews;
using Microsoft.AspNetCore.SignalR;

namespace Bugget.Hubs;

public class ReportPageHubClient(IHubContext<ReportPageHub> hubContext) : IReportPageHubClient
{
    private readonly IHubContext<ReportPageHub> _hubContext = hubContext;

    public Task SendReportPatchAsync(int reportId, PatchReportSocketView view)
    {
        return _hubContext.Clients.Group($"{reportId}")
            .SendAsync("ReceiveReportPatch", view);
    }

    public Task SendReportParticipantsAsync(int reportId, string[] participants)
    {
        return _hubContext.Clients.Group($"{reportId}")
            .SendAsync("ReceiveReportParticipants", participants);
    }
} 