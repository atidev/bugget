using Monade.Errors;

namespace Bugget.BO.Errors;

public static class BoErrors
{
    public static readonly NotFoundError BugNotFoundError = new NotFoundError("bug_not_found", "Баг не найден");
    public static readonly NotFoundError ReportNotFoundError = new NotFoundError("report_not_found", "Репорт не найден");
    public static readonly InternalServerError InternalServerError = new InternalServerError("internal_server_error", "Внутреняя ошибка сервера");
}