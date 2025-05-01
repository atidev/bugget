using Bugget.DA.Files;
using Bugget.DA.Interfaces;
using Bugget.Entities.Adapters;
using Bugget.ExternalClients.Context;
using Bugget.ExternalClients.Interfaces;

namespace Bugget.ExternalClients.Notifications.Mattermost;

public sealed class MattermostService(
    IEmployeesClient employeesClient,
    MattermostClient mattermostClient) : IReportCreatePostAction, IReportUpdatePostAction, IReportPatchPostAction
{
    public async Task ExecuteAsync(ReportCreateContext createContext)
    {
        if (createContext.Report.ResponsibleUserId == createContext.ReportDbModel.CreatorUserId)
            return;

        var responsibleEmployee = await employeesClient.GetEmployeeAsync(createContext.Report.ResponsibleUserId);
        var creatorEmployee = await employeesClient.GetEmployeeAsync(createContext.Report.CreatorUserId);
        if (responsibleEmployee == null || creatorEmployee == null)
            return ;

        var message = ReportMessageBuilder.GetYourResponsibleInNewReportMessage(
            createContext.ReportDbModel.Id, createContext.ReportDbModel.Title, creatorEmployee.Name
        );

        await mattermostClient.SendMessageAsync(responsibleEmployee.NotificationUserId, message);
    }

    public async Task ExecuteAsync(ReportUpdateContext createContext)
    {
        // не меняля ответственного
        if (createContext.Report.ResponsibleUserId == null)
            return;

        if (createContext.Report.UpdaterUserId == createContext.Report.ResponsibleUserId)
            return ;

        var responsibleEmployee = await employeesClient.GetEmployeeAsync(createContext.Report.ResponsibleUserId);
        var updaterEmployee = await employeesClient.GetEmployeeAsync(createContext.Report.UpdaterUserId);
        if (responsibleEmployee == null || updaterEmployee == null)
            return ;

        var message = ReportMessageBuilder.GetYourResponsibleInExistReportMessage(
            createContext.ReportDbModel.Id, createContext.ReportDbModel.Title, updaterEmployee.Name
        );

        await mattermostClient.SendMessageAsync(responsibleEmployee.NotificationUserId, message);
    }

    public async Task ExecuteAsync(ReportPatchContext reportPatchContext)
    {
        // не меняли ответственного
        if (reportPatchContext.PatchDto.ResponsibleUserId == null)
            return;

        if (reportPatchContext.UserId == reportPatchContext.Result.ResponsibleUserId)
            return;

        var responsibleEmployee = await employeesClient.GetEmployeeAsync(reportPatchContext.Result.ResponsibleUserId);
        var updaterEmployee = await employeesClient.GetEmployeeAsync(reportPatchContext.UserId);
        if (responsibleEmployee == null || updaterEmployee == null)
            return;

        var message = ReportMessageBuilder.GetYourResponsibleAfterPatchReportMessage(
            reportPatchContext.Result.Id, reportPatchContext.Result.Title, updaterEmployee.Name
        );

        await mattermostClient.SendMessageAsync(responsibleEmployee.NotificationUserId, message);
    }
}