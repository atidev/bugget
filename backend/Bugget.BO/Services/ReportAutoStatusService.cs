using Bugget.DA.Postgres;
using Bugget.DA.WebSockets;
using Bugget.Entities.BO.ReportBo;
using Bugget.Entities.DbModels.Report;
using Bugget.Entities.DTO.Report;
using Bugget.Entities.SocketViews;

namespace Bugget.BO.Services
{
    public class ReportAutoStatusService(ReportsDbClient reportsDbClient, IReportPageHubClient reportPageHubClient)
    {
        public async Task CalculateStatusAsync(int reportId, ReportPatchDto patchDto, ReportPatchResultDbModel result)
        {
            // если статус backlog и меняется ответственный, то меняем статус на in progress
            if (result.Status == (int)ReportStatus.Backlog && patchDto.ResponsibleUserId != null)
            {
                await reportsDbClient.ChangeStatusAsync(reportId, (int)ReportStatus.InProgress);
                await reportPageHubClient.SendReportPatchAsync(reportId, new PatchReportSocketView
                {
                    Status = (int)ReportStatus.InProgress,
                }, null);
            }
        }
    }
}