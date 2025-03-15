namespace Bugget.Features.Notifications;

public static class ReportMessageBuilder
{
    public static string GetYourResponsibleInNewReportMessage(int reportId, string reportTitle, string creatorUserFullName)
    {
        // TODO add domain path like https://inner.domain.com/
        return $":plus-black: Вы назначены ответственным **[в баг-репорте](http://inner.domain.com/reports/{reportId})**\n" +
               $"Название: **{reportTitle}**\n" +
               $"Создатель: {creatorUserFullName}";
    }
    
    public static string GetYourResponsibleInExistReportMessage(int reportId, string reportTitle, string creatorUserFullName)
    {
        // TODO add domain path like https://inner.domain.com/
        return $":recycle: Вы назначены ответственным **[в баг-репорте](https://inner.domain.com/reports/{reportId})**\n" +
               $"Название: **{reportTitle}**\n" +
               $"Инициатор: {creatorUserFullName}";
    }
}