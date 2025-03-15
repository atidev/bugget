using Microsoft.AspNetCore.SignalR;

namespace Bugget.Hubs;

public sealed class ReportPageHub: Hub
{
    // Подключение к группе комментариев по reportId
    public async Task JoinReportGroupAsync(int reportId)
    {
        Console.WriteLine($"✅ Клиент {Context.ConnectionId} подключился к группе {reportId}");
        await Groups.AddToGroupAsync(Context.ConnectionId, $"{reportId}");
    }

    // Отключение от группы
    public async Task LeaveReportGroupAsync(int reportId)
    {
        Console.WriteLine($"\ud83d\udeaa Клиент {Context.ConnectionId} покинул группу {reportId}");
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"{reportId}");
    }
    
    // Логирование разрыва соединения
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        Console.WriteLine($"⚠️ Клиент {Context.ConnectionId} отключился. Причина: {exception?.Message ?? "неизвестно"}");
        await base.OnDisconnectedAsync(exception);
    }
}