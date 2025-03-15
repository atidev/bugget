using Bugget.Features.Notifications.Mattermost;

namespace Bugget.Features.Notifications;

public static class ReportMessageBuilder
{
    private static readonly string? BuggetBaseUrl = Environment.GetEnvironmentVariable(MattermostConstants.BuggetBaseUrlKey);

    public static string GetYourResponsibleInNewReportMessage(int reportId, string reportTitle, string creatorUserFullName)
    {
        return $":plus-black: Вы назначены ответственным **[в баг-репорте]({BuggetBaseUrl}/reports/{reportId})**\n" +
               $"Название: **{reportTitle}**\n" +
               $"Создатель: {creatorUserFullName}";
    }
    
    public static string GetYourResponsibleInExistReportMessage(int reportId, string reportTitle, string creatorUserFullName)
    {
        return $":recycle: Вы назначены ответственным **[в баг-репорте]({BuggetBaseUrl}/reports/{reportId})**\n" +
               $"Название: **{reportTitle}**\n" +
               $"Инициатор: {creatorUserFullName}";
    }
}