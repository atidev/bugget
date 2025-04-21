using Bugget.ExternalClients.Context;

namespace Bugget.ExternalClients.Interfaces;

public interface IReportPatchPostAction
{
    public Task ExecuteAsync(ReportPatchContext reportPatchContext);
}