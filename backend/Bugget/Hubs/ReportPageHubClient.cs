using Bugget.BO.WebSockets;
using Bugget.Entities.SocketViews;
using Microsoft.AspNetCore.SignalR;

namespace Bugget.Hubs;

public class ReportPageHubClient : IReportPageHubClient
{
    private readonly IHubContext<ReportPageHub> _hubContext;

    public ReportPageHubClient(IHubContext<ReportPageHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public Task SendReportPatchAsync(int reportId, PatchReportSocketView view)
    {
        return _hubContext.Clients.Group($"{reportId}")
            .SendAsync("ReceiveReportPatch", view);
    }
} 