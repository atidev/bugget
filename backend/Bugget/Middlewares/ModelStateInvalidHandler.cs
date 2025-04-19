using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Bugget.Middlewares;

public sealed class ModelStateInvalidHandler : IActionResult
{
    private const string Error = "model_state_validation_error";
    private const string Reason = "Ошибка валидации модели запроса";
    private const string LogKey = "errorList";

    private readonly LogLevel _logLevel;

    public ModelStateInvalidHandler(LogLevel logLevel = LogLevel.Warning)
    {
        _logLevel = logLevel;
    }

    public Task ExecuteResultAsync(ActionContext context)
    {
        var modelState = context.ModelState;
        if (modelState.IsValid || modelState.Count == 0)
            return Task.CompletedTask;

        var errors = modelState
            .Where(p => p.Value?.Errors.Count > 0)
            .SelectMany(GetFieldErrors!).ToArray();

        var httpContext = context.HttpContext;

        var logger = httpContext.RequestServices.GetService<ILogger<ModelStateInvalidHandler>>();

        if (logger != null && logger.IsEnabled(_logLevel))
        {
            using (logger.BeginScope(new Dictionary<string, object> { { LogKey, errors } }))
            {
                logger.Log(_logLevel, Reason);
            }
        }

        httpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        return httpContext.Response.WriteAsJsonAsync(new MultipleError
        (
            Error,
            Reason,
            errors
        ));
    }

    private static IEnumerable<MultipleErrorElement> GetFieldErrors(KeyValuePair<string, ModelStateEntry> field)
    {
        var (name, value) = field;
        return value.Errors.Select(e => new MultipleErrorElement(name, e.ErrorMessage));
    }

    private record MultipleError(string Error, string Reason, MultipleErrorElement[] ErrorList);

    private record MultipleErrorElement(string Error, string Reason);
}