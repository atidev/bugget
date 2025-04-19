using System.Collections;
using Microsoft.AspNetCore.Mvc;
using Monade;

namespace Bugget.Extensions
{
    public static class ResultExtensions
    {
        public static async Task<IActionResult> AsActionResultAsync(
        this Task<MonadeStruct> operationTask,
        int successStatusCode = 200)
        {
            var operation = await operationTask;

            return operation.AsActionResult(successStatusCode);
        }

        public static async Task<IActionResult> AsActionResultAsync<TValue>(
            this Task<MonadeStruct<TValue>> operationTask,
            int successStatusCode = 200)
        {
            var operation = await operationTask;

            return operation.AsActionResult(successStatusCode);
        }

        public static async Task<IActionResult> AsActionResultAsync<TValue, TView>(
            this Task<MonadeStruct<TValue>> operationTask,
            Func<TValue, TView> toView,
            int successStatusCode = 200)
        {
            var operation = await operationTask;

            return operation.AsActionResult(toView, successStatusCode);
        }

        public static IActionResult AsActionResult(
            this MonadeStruct operation,
            int successStatusCode = 200)
        {
            if (operation.IsSuccess)
                return new StatusCodeResult(successStatusCode);

            return new JsonResult(operation.Error)
            {
                StatusCode = operation.Error!.ExtractStatusCode()
            };
        }

        public static IActionResult AsActionResult<TValue>(
            this MonadeStruct<TValue> operation,
            int successStatusCode = 200)
        {
            if (operation.IsSuccess)
                return new JsonResult(operation.Value)
                {
                    StatusCode = successStatusCode
                };

            return new JsonResult(operation.Error)
            {
                StatusCode = operation.Error!.ExtractStatusCode()
            };
        }

        public static IActionResult AsActionResult<TValue, TView>(
            this MonadeStruct<TValue> operation,
            Func<TValue, TView> toView,
            int successStatusCode = 200)
        {
            if (operation.HasError)
                return new JsonResult(operation.Error)
                {
                    StatusCode = operation.Error!.ExtractStatusCode()
                };

            if (operation.Value == null)
                return new JsonResult(ConvertNullToContract(typeof(TValue)))
                {
                    StatusCode = successStatusCode
                };

            return new JsonResult(toView(operation.Value))
            {
                StatusCode = successStatusCode
            };
        }

        private static readonly object EmptyObject = new { };

        private static object ConvertNullToContract(Type type)
        {
            var typeIsArray = typeof(ICollection).IsAssignableFrom(type);
            var typeIsDictionary = typeof(IDictionary).IsAssignableFrom(type);
            if (typeIsArray && !typeIsDictionary)
                return Array.Empty<object>();

            return EmptyObject;
        }
    }
}