using Bugget.Features.Context;

namespace Bugget.Features.Interfaces;

public interface IReportUpdatePostAction
{
    public Task ExecuteAsync(ReportUpdateContext createContext);
}