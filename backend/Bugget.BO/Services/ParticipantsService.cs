using Bugget.DA.Postgres;
using Bugget.DA.WebSockets;

namespace Bugget.BO.Services
{
    public class ParticipantsService(ParticipantsDbClient participantsDbClient, IReportPageHubClient reportPageHubClient)
    {
        public async Task AddParticipantIfNotExistAsync(int reportId, string userId)
        {
            var participants = await participantsDbClient.AddParticipantIfNotExistAsync(reportId, userId);

            if (participants != null)
            {
                await reportPageHubClient.SendReportParticipantsAsync(reportId, participants);
            }
        }
    }
}