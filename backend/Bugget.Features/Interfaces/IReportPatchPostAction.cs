using Bugget.Features.Context;

namespace Bugget.Features.Interfaces;

public interface IReportPatchPostAction
{
    public Task ExecuteAsync(ReportPatchContext reportPatchContext);
}