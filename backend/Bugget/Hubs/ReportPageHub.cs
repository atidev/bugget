using Bugget.Authentication;
using Bugget.BO.Services;
using Bugget.Entities.DTO.Report;
using Microsoft.AspNetCore.SignalR;

namespace Bugget.Hubs;

public sealed class ReportPageHub(
    ILogger<ReportPageHub> logger,
    ReportsService reportsService) : Hub
{
    // Подключение к группе комментариев по reportId
    public async Task JoinReportGroupAsync(int reportId)
    {
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

    public Task PatchReportAsync(int reportId, ReportPatchDto patchDto)
    {
        if (Context.User == null)
        {
            logger.LogError("Пользователь не авторизован");
            return Task.CompletedTask;
        }

        var user = new UserIdentity(Context.User);
        return reportsService.PatchReportAsync(
            reportId,
            user.Id,
            user.OrganizationId,
            patchDto
        );
    }
}