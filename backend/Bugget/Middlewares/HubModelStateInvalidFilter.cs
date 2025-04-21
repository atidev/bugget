using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.SignalR;

namespace Bugget.Middlewares
{
    public class HubModelStateInvalidFilter : IHubFilter
    {
        public async ValueTask<object?> InvokeMethodAsync(
            HubInvocationContext invocationContext,
            Func<HubInvocationContext, ValueTask<object?>> next)
        {
            // Проходим по всем аргументам метода
            foreach (var arg in invocationContext.HubMethodArguments)
            {
                if (arg == null) continue;
                var validationContext = new ValidationContext(arg);
                var errors = new List<ValidationResult>();
                // Проверяем только объекты с атрибутами
                if (!Validator.TryValidateObject(arg, validationContext, errors, validateAllProperties: true))
                {
                    var msg = string.Join("; ", errors.Select(e => e.ErrorMessage));
                    throw new HubException($"Validation failed: {msg}");
                }
            }
            return await next(invocationContext);
        }
    }
}