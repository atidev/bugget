using Bugget.ExternalClients.Context;
using Bugget.ExternalClients.Interfaces;

namespace Bugget.ExternalClients;

public sealed class ExternalClientsActionService(
    IEnumerable<IReportPatchPostAction> reportPatchPostActions)
{
    public async Task ExecuteReportPatchPostActions(ReportPatchContext reportPatchContext)
    {
        foreach (var reportPatchPostAction in reportPatchPostActions)
        {
            await reportPatchPostAction.ExecuteAsync(reportPatchContext);
        }
    }
}