using Bugget.ExternalClients.Context;

namespace Bugget.ExternalClients.Interfaces;

public interface IReportUpdatePostAction
{
    public Task ExecuteAsync(ReportUpdateContext createContext);
}