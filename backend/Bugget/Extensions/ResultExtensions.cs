using System.Collections;
using Monade;

namespace Bugget.Extensions
{
    public static class ResultExtensions
    {
        public static async Task<IResult> AsResultAsync(
            this Task<MonadeStruct> operationTask,
            int successStatusCode = 200)
        {
            var operation = await operationTask;

            return operation.AsResult(successStatusCode);
        }

        public static async Task<IResult> AsResultAsync<TValue>(
            this Task<MonadeStruct<TValue>> operationTask,
            int successStatusCode = 200)
        {
            var operation = await operationTask;

            return operation.AsResult(successStatusCode);
        }

        public static async Task<IResult> AsResultAsync<TValue, TView>(
            this Task<MonadeStruct<TValue>> operationTask,
            Func<TValue, TView> toView,
            int successStatusCode = 200)
        {
            var operation = await operationTask;

            return operation.AsResult(toView, successStatusCode);
        }

        public static IResult AsResult(
            this MonadeStruct operation,
            int successStatusCode = 200)
        {
            if (operation.IsSuccess)
                return Results.StatusCode(successStatusCode);

            return Results.Json(operation.Error, statusCode: operation.Error!.ExtractStatusCode());
        }

        public static IResult AsResult<TValue>(
            this MonadeStruct<TValue> operation,
            int successStatusCode = 200)
        {
            if (operation.IsSuccess)
                return Results.Json(operation.Value, statusCode: successStatusCode);

            return Results.Json(operation.Error, statusCode: operation.Error!.ExtractStatusCode());
        }

        public static IResult AsResult<TValue, TView>(
            this MonadeStruct<TValue> operation,
            Func<TValue, TView> toView,
            int successStatusCode = 200)
        {
            if (operation.HasError)
                return Results.Json(operation.Error, statusCode: operation.Error!.ExtractStatusCode());

            if (operation.Value == null)
                return Results.Json(ConvertNullToContract(typeof(TValue)), statusCode: successStatusCode);

            return Results.Json(toView(operation.Value), statusCode: successStatusCode);
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