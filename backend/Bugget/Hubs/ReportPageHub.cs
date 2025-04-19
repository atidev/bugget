using Bugget.Authentication;
using Bugget.BO.Services;
using Bugget.Entities.DTO.Report;
using Bugget.Entities.SocketView;
using Bugget.Features;
using Bugget.Features.Context;
using Microsoft.AspNetCore.SignalR;

namespace Bugget.Hubs;

public sealed class ReportPageHub(
    ILogger<ReportPageHub> logger,
    ReportsService reportsService,
    FeaturesService featuresService) : Hub
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

    public async Task PatchReportAsync(int reportId, ReportPatchDto patchDto)
    {
        if (Context.User == null)
        {
            logger.LogError("Пользователь не авторизован");
            return;
        }

        var user = new UserIdentity(Context.User);

        logger.LogInformation("Пользователь {@UserId} патчит отчёт {@ReportId}, {@PatchDto}", user.Id, reportId, patchDto);

        var result = await reportsService.PatchReportAsync(
            reportId,
            user.Id,
            user.OrganizationId,
            patchDto
        );

        var socketView = new ReportSocketView
        {
            Title = patchDto.Title,
            Status = patchDto.Status,
            ResponsibleUserId = patchDto.ResponsibleUserId,
            PastResponsibleUserId = patchDto.ResponsibleUserId != null ? result.PastResponsibleUserId : null,
            ParticipantsUserIds = result.IsParticipantsChanged ? result.ParticipantsUserIds : null,
        };

        await Clients.Group($"{reportId}")
            .SendAsync("ReceiveReportPatch", socketView);

        await featuresService.ExecuteReportPatchPostActions(new ReportPatchContext(user.Id, patchDto, result));
    }
}