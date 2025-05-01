using Bugget.Entities.Authentication;
using Bugget.DA.WebSockets;
using Bugget.Entities.DbModels.Bug;
using Bugget.Entities.DTO.Bug;

namespace Bugget.BO.Services
{
    public class BugsEventsService(
            IReportPageHubClient reportPageHubClient,
            ParticipantsService participantsService)
    {
        public async Task HandleCreateBugEventAsync(int reportId, UserIdentity user, BugSummaryDbModel result)
        {
            await Task.WhenAll(
                reportPageHubClient.SendBugCreateAsync(reportId, result, user.SignalRConnectionId),
                participantsService.AddParticipantIfNotExistAsync(reportId, user.Id)
            );
        }

        public async Task HandlePatchBugEventAsync(int reportId, int bugId, UserIdentity user, BugPatchDto patchDto)
        {
            await Task.WhenAll(
                reportPageHubClient.SendBugPatchAsync(reportId, bugId, patchDto, user.SignalRConnectionId),
                participantsService.AddParticipantIfNotExistAsync(reportId, user.Id)
            );
        }
    }
}