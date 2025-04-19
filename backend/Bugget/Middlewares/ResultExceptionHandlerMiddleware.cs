using System.Net;
using Bugget.BO.Errors;
using Monade;
using Monade.Errors;
using Npgsql;

namespace Bugget.Middlewares;

public class ResultExceptionHandlerMiddleware(ILogger<ResultExceptionHandlerMiddleware> logger) : IMiddleware
{
    private const string CatchedOnMiddlewareError = "Обработана ошибка на мидлваре";
    private readonly ILogger _logger = logger;

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (PostgresException ex) when (ex.SqlState == "P404")
        {
            context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            await context.Response.WriteAsJsonAsync(BoErrors.NotFoundError);
        }
        catch (Exception e)
        {
            _logger.LogError(e, CatchedOnMiddlewareError);

            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            await context.Response.WriteAsJsonAsync(BoErrors.InternalServerError);
        }
    }
}