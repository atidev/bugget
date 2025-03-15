using Bugget.Features.Context;
using Bugget.Features.Interfaces;

namespace Bugget.Features;

public class WithoutFeaturesService : IReportCreatePostAction, IReportUpdatePostAction
{
    public Task ExecuteAsync(ReportCreateContext createContext)
    {
        return Task.CompletedTask;
    }

    public Task ExecuteAsync(ReportUpdateContext createContext)
    {
        return Task.CompletedTask;
    }
}