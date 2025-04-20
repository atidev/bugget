using Bugget.BO.Mappers;
using Bugget.BO.WebSockets;
using Bugget.Entities.DbModels.Report;
using Bugget.Entities.DTO.Report;
using Bugget.ExternalClients;
using Bugget.ExternalClients.Context;

namespace Bugget.BO.Services
{
    public class ReportEventsService(
        IReportPageHubClient reportPageHubClient,
        ExternalClientsActionService externalClientsActionService,
        ParticipantsService participantsService,
        ReportAutoStatusService autoStatusService)
    {
        public async Task HandlePatchReportEventAsync(int reportId, string userId, ReportPatchDto patchDto, ReportPatchResultDbModel result)
        {
            await Task.WhenAll(
                reportPageHubClient.SendReportPatchAsync(reportId, patchDto.ToSocketView(result)),
                externalClientsActionService.ExecuteReportPatchPostActions(new ReportPatchContext(userId, patchDto, result)),
                participantsService.AddParticipantIfNotExistAsync(reportId, userId),
                autoStatusService.CalculateStatusAsync(reportId, patchDto, result)
            );
        }
    }
}