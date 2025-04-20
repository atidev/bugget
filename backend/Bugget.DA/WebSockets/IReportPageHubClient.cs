using Bugget.Entities.SocketViews;

namespace Bugget.BO.WebSockets;

public interface IReportPageHubClient
{
    Task SendReportPatchAsync(int reportId, PatchReportSocketView view);
}