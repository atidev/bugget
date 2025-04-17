using System.Net;
using Npgsql;
using Result;
using Result.Errors;

public class ResultExceptionHandlerMiddleware : IMiddleware
{
    private const string CatchedOnMiddlewareError = "Обработана ошибка на мидлваре";
    private static readonly Error InternalServerError = new InternalServerError("internal_server_error", "Внутренняя ошибка сервера");
    private static readonly Error NotFoundError = new NotFoundError("not_found", "Объект не найден");

    private readonly ILogger _logger;

    public ResultExceptionHandlerMiddleware(ILogger<ResultExceptionHandlerMiddleware> logger)
    {
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (PostgresException ex) when (ex.SqlState == "P0404")
        {
            context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            await context.Response.WriteAsJsonAsync(NotFoundError);
        }
        catch (Exception e)
        {
            _logger.LogError(e, CatchedOnMiddlewareError);

            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            await context.Response.WriteAsJsonAsync(InternalServerError);
        }
    }
}