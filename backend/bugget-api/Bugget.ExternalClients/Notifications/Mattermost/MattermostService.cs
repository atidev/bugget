using Bugget.DA.Interfaces;
using Bugget.ExternalClients.Context;
using Bugget.ExternalClients.Interfaces;

namespace Bugget.ExternalClients.Notifications.Mattermost;

public sealed class MattermostService(
    IUsersClient usersClient,
    MattermostClient mattermostClient) : IReportPatchPostAction
{
    public async Task ExecuteAsync(ReportPatchContext reportPatchContext)
    {
        // не меняли ответственного
        if (reportPatchContext.PatchDto.ResponsibleUserId == null)
            return;

        if (reportPatchContext.UserId == reportPatchContext.Result.ResponsibleUserId)
            return;

        var responsibleUser = await usersClient.GetUserAsync(reportPatchContext.Result.ResponsibleUserId);
        var updaterUser = await usersClient.GetUserAsync(reportPatchContext.UserId);
        if (responsibleUser == null || updaterUser == null)
            return;

        var message = ReportMessageBuilder.GetYourResponsibleAfterPatchReportMessage(
            reportPatchContext.Result.Id, reportPatchContext.Result.Title, updaterUser.Name
        );

        await mattermostClient.SendMessageAsync(responsibleUser.NotificationUserId, message);
    }
}