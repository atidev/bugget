using Bugget.Features.Context;
using Bugget.Features.Interfaces;
using Bugget.Features.TaskQueue;

namespace Bugget.Features;

public sealed class FeaturesService(
    IEnumerable<IReportCreatePostAction> reportCreatePostActions,
    IEnumerable<IReportUpdatePostAction> reportUpdatePostActions,
    ITaskQueue taskQueue)
{
    public async Task ExecuteReportCreatePostActions(ReportCreateContext reportCreateContext)
    {
        foreach (var reportCreatePostAction in reportCreatePostActions)
        {
            await taskQueue.Enqueue(() =>
                reportCreatePostAction.ExecuteAsync(reportCreateContext));
        }
    }
    
    public async Task ExecuteReportUpdatePostActions(ReportUpdateContext reportUpdateContext)
    {
        foreach (var reportUpdatePostAction in reportUpdatePostActions)
        {
            await taskQueue.Enqueue(() =>
                reportUpdatePostAction.ExecuteAsync(reportUpdateContext));
        }
    }
}