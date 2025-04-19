using Monade;
using Monade.Errors;
using System.Net;

namespace Bugget.Extensions
{
    public static class ErrorExtensions
    {
        public static int ExtractStatusCode(this Error error) => (int)(error switch
        {
            BadRequestError => HttpStatusCode.BadRequest,
            NotFoundError => HttpStatusCode.NotFound,
            InternalServerError => HttpStatusCode.InternalServerError,
            _ => throw new NotImplementedException("Данный тип ошибки не определен")
        });
    }
}