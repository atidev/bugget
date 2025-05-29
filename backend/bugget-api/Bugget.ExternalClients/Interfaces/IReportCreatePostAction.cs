using Bugget.ExternalClients.Context;

namespace Bugget.ExternalClients.Interfaces;

public interface IReportCreatePostAction
{
    public Task ExecuteAsync(ReportCreateContext createContext);
}