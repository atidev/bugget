using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;

namespace Bugget.Hubs
{
    public class SignalRUserIdProvider : IUserIdProvider
    {
        public string GetUserId(HubConnectionContext connection)
            => connection.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
    }
}