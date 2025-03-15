using Bugget.Features.Context;

namespace Bugget.Features.Interfaces;

public interface IReportCreatePostAction
{
    public Task ExecuteAsync(ReportCreateContext createContext);
}