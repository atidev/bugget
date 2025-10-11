using Bugget.DA.Files;
using Bugget.DA.Interfaces;
using Bugget.Entities.Adapters;
using Bugget.ExternalClients.Context;
using Bugget.ExternalClients.Interfaces;

namespace Bugget.ExternalClients.Notifications.Mattermost;

public sealed class MattermostService(
    IUsersClient usersClient,
    MattermostClient mattermostClient) : IReportCreatePostAction, IReportUpdatePostAction, IReportPatchPostAction
{
    public async Task ExecuteAsync(ReportCreateContext createContext)
    {
        if (createContext.Report.ResponsibleUserId == createContext.ReportDbModel.CreatorUserId)
            return;

        var responsibleUser = await usersClient.GetUserAsync(createContext.Report.ResponsibleUserId);
        var creatorUser = await usersClient.GetUserAsync(createContext.Report.CreatorUserId);
        if (responsibleUser == null || creatorUser == null)
            return ;

        var message = ReportMessageBuilder.GetYourResponsibleInNewReportMessage(
            createContext.ReportDbModel.Id, createContext.ReportDbModel.Title, creatorUser.Name
        );

        await mattermostClient.SendMessageAsync(responsibleUser.NotificationUserId, message);
    }

    public async Task ExecuteAsync(ReportUpdateContext createContext)
    {
        // не меняля ответственного
        if (createContext.Report.ResponsibleUserId == null)
            return;

        if (createContext.Report.UpdaterUserId == createContext.Report.ResponsibleUserId)
            return ;

        var responsibleUser = await usersClient.GetUserAsync(createContext.Report.ResponsibleUserId);
        var updaterUser = await usersClient.GetUserAsync(createContext.Report.UpdaterUserId);
        if (responsibleUser == null || updaterUser == null)
            return ;

        var message = ReportMessageBuilder.GetYourResponsibleInExistReportMessage(
            createContext.ReportDbModel.Id, createContext.ReportDbModel.Title, updaterUser.Name
        );

        await mattermostClient.SendMessageAsync(responsibleUser.NotificationUserId, message);
    }

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