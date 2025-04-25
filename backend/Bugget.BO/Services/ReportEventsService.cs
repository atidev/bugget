using Authentication;
using Bugget.BO.Mappers;
using Bugget.DA.WebSockets;
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
        public async Task HandlePatchReportEventAsync(int reportId, UserIdentity user, ReportPatchDto patchDto, ReportPatchResultDbModel result)
        {
            await Task.WhenAll(
                reportPageHubClient.SendReportPatchAsync(reportId, patchDto.ToSocketView(result), user.SignalRConnectionId),
                externalClientsActionService.ExecuteReportPatchPostActions(new ReportPatchContext(user.Id, patchDto, result)),
                participantsService.AddParticipantIfNotExistAsync(reportId, user.Id),
                patchDto.ResponsibleUserId != null ? participantsService.AddParticipantIfNotExistAsync(reportId, patchDto.ResponsibleUserId) : Task.CompletedTask,
                autoStatusService.CalculateStatusAsync(reportId, patchDto, result)
            );
        }
    }
}