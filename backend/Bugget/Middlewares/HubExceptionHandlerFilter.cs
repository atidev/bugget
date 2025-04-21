using System.Text.Json;
using Bugget.BO.Errors;
using Microsoft.AspNetCore.SignalR;
using Npgsql;

namespace Bugget.Middlewares;

public class HubExceptionHandlerFilter(ILogger<HubExceptionHandlerFilter> logger) : IHubFilter
{
    protected readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
    };

    public async ValueTask<object?> InvokeMethodAsync(
        HubInvocationContext context,
        Func<HubInvocationContext, ValueTask<object?>> next)
    {
        try
        {
            return await next(context);
        }
        catch (PostgresException ex) when (ex.SqlState == "P0404")
        {
            throw new HubException(JsonSerializer.Serialize(BoErrors.NotFoundError, JsonSerializerOptions));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception in hub method {HubMethod}", context.HubMethodName);
            throw new HubException(JsonSerializer.Serialize(BoErrors.InternalServerError, JsonSerializerOptions));
        }
    }
}