using Monade.Errors;

namespace Bugget.BO.Errors;

public static class BoErrors
{
    public static readonly NotFoundError BugNotFoundError = new NotFoundError("bug_not_found", "Баг не найден");
    public static readonly NotFoundError NotFoundError = new NotFoundError("not_found", "Объект не найден");
    public static readonly NotFoundError ReportNotFoundError = new NotFoundError("report_not_found", "Репорт не найден");
    public static readonly InternalServerError InternalServerError = new InternalServerError("internal_server_error", "Внутреняя ошибка сервера");
    public static readonly BadRequestError BugMustHaveOneField = new BadRequestError("bug_must_have_one_field", "Баг должен содержать хотя бы одно поле");
}