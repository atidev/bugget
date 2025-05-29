using Monade.Errors;

namespace Bugget.BO.Errors;

public static class BoErrors
{
    public static readonly NotFoundError NotFoundError = new NotFoundError("not_found", "Объект не найден");
    public static readonly NotFoundError ReportNotFoundError = new NotFoundError("report_not_found", "Репорт не найден");
    public static readonly InternalServerError InternalServerError = new InternalServerError("internal_server_error", "Внутреняя ошибка сервера");
    public static readonly BadRequestError BugMustHaveOneField = new BadRequestError("bug_must_have_one_field", "Баг должен содержать хотя бы одно поле");

    public static readonly BadRequestError AttachmentFileNotSelectedOrEmpty = new BadRequestError("attachment_file_not_selected_or_empty", "Файл не выбран или пуст.");
    public static readonly BadRequestError AttachmentFileTooLarge = new BadRequestError("attachment_file_too_large", "Файл превышает допустимый размер 10 МБ.");
    public static readonly BadRequestError AttachmentFileExtensionNotFound = new BadRequestError("attachment_file_extension_not_found", "Расширение файла не найдено.");
    public static readonly BadRequestError AttachmentFileNameInvalidChars = new BadRequestError("attachment_file_name_invalid_chars", "Имя файла содержит недопустимые символы.");
    public static readonly BadRequestError AttachmentLimitExceeded = new BadRequestError("attachment_limit_exceeded", "Превышен лимит количества файлов.");
    public static readonly BadRequestError AttachmentTypeNotAllowed = new BadRequestError("attachment_type_not_allowed", "Недопустимый тип файла.");
    public static readonly NotFoundError AttachmentNotFound = new NotFoundError("attachment_not_found", "Файл не найден.");

    public static readonly NotFoundError UserCommentNotFound = new NotFoundError("user_comment_not_found", "Комментарий пользователя не найден.");
}