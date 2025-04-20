using Bugget.ExternalClients.Context;
using Bugget.ExternalClients.Interfaces;

namespace Bugget.ExternalClients;

public sealed class ExternalClientsActionService(
    IEnumerable<IReportCreatePostAction> reportCreatePostActions,
    IEnumerable<IReportUpdatePostAction> reportUpdatePostActions,
    IEnumerable<IReportPatchPostAction> reportPatchPostActions)
{
    public async Task ExecuteReportCreatePostActions(ReportCreateContext reportCreateContext)
    {
        foreach (var reportCreatePostAction in reportCreatePostActions)
        {
            await reportCreatePostAction.ExecuteAsync(reportCreateContext);
        }
    }
    
    public async Task ExecuteReportUpdatePostActions(ReportUpdateContext reportUpdateContext)
    {
        foreach (var reportUpdatePostAction in reportUpdatePostActions)
        {
            await reportUpdatePostAction.ExecuteAsync(reportUpdateContext);
        }
    }

    public async Task ExecuteReportPatchPostActions(ReportPatchContext reportPatchContext)
    {
        foreach (var reportPatchPostAction in reportPatchPostActions)
        {
            await reportPatchPostAction.ExecuteAsync(reportPatchContext);
        }
    }
}