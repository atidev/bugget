using Bugget.DA.Files;
using Bugget.Entities.Mappers;
using Bugget.Features.Context;
using Bugget.Features.Interfaces;

namespace Bugget.Features.Notifications.Mattermost;

public sealed class MattermostService(
    EmployeesDataAccess employeesDataAccess,
    MattermostClient mattermostClient) : IReportCreatePostAction, IReportUpdatePostAction
{
    public Task ExecuteAsync(ReportCreateContext createContext)
    {
        if (createContext.Report.ResponsibleUserId == createContext.ReportDbModel.CreatorUserId)
            return Task.CompletedTask;

        var responsibleEmployee = employeesDataAccess.GetEmployee(createContext.Report.ResponsibleUserId);
        var creatorEmployee = employeesDataAccess.GetEmployee(createContext.Report.CreatorUserId);
        if (responsibleEmployee == null || creatorEmployee == null)
            return Task.CompletedTask;

        var message = ReportMessageBuilder.GetYourResponsibleInNewReportMessage(
            createContext.ReportDbModel.Id, createContext.ReportDbModel.Title, EmployeesMapper.Transform(creatorEmployee).Name
        );

        return mattermostClient.SendMessageAsync(responsibleEmployee.NotificationUserId, message);
    }

    public Task ExecuteAsync(ReportUpdateContext createContext)
    {
        // не меняля ответственного
        if (createContext.Report.ResponsibleUserId == null)
            return Task.CompletedTask;

        if (createContext.Report.UpdaterUserId == createContext.Report.ResponsibleUserId)
            return Task.CompletedTask;

        var responsibleEmployee = employeesDataAccess.GetEmployee(createContext.Report.ResponsibleUserId);
        var updaterEmployee = employeesDataAccess.GetEmployee(createContext.Report.UpdaterUserId);
        if (responsibleEmployee == null || updaterEmployee == null)
            return Task.CompletedTask;

        var message = ReportMessageBuilder.GetYourResponsibleInExistReportMessage(
            createContext.ReportDbModel.Id, createContext.ReportDbModel.Title, EmployeesMapper.Transform(updaterEmployee).Name
        );

        return mattermostClient.SendMessageAsync(responsibleEmployee.NotificationUserId, message);
    }
}