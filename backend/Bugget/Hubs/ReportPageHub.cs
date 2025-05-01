using Microsoft.AspNetCore.SignalR;
using Bugget.BO.Services;
using Bugget.Entities.Authentication;
namespace Bugget.Hubs;

public sealed class ReportPageHub(
    ILogger<ReportPageHub> logger,
    ReportsService reportsService) : Hub
{
    // Подключение к группе комментариев по reportId
    public async Task JoinReportGroupAsync(int reportId)
    {
        var user = Context.User?.GetIdentity();
        if (user is null)
        {
            throw new HubException("пользователь не авторизован");
        }

        if (!await reportsService.GetReportAccessAsync(reportId, user.OrganizationId))
        {
            throw new HubException("отчет не найден");
        }

        logger.LogInformation("Клиент {@ConnectionId} подключился к группе {@ReportId}", Context.ConnectionId, reportId);
        await Groups.AddToGroupAsync(Context.ConnectionId, $"{reportId}");
    }

    // Отключение от группы
    public async Task LeaveReportGroupAsync(int reportId)
    {
        logger.LogInformation("Клиент {@ConnectionId} покинул группу {@ReportId}", Context.ConnectionId, reportId);
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"{reportId}");
    }

    // Логирование разрыва соединения
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        logger.LogInformation("Клиент {@ConnectionId} отключился. Причина: {@Reason}", Context.ConnectionId, exception?.Message ?? "неизвестно");
        await base.OnDisconnectedAsync(exception);
    }
}