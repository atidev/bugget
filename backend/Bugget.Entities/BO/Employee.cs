
namespace Bugget.Entities.BO;

public class Employee
{
    /// <summary>
    /// Уникальный идентификатор соответствующий хэдеру авторизации USER_ID_KEY
    /// </summary>
    public required string Id { get; init; }

    private readonly string? _name;

    /// <summary>
    /// Имя пользователя. Если значение не задано или пустое — возвращает Id.
    /// </summary>
    public string Name
    {
        get => string.IsNullOrEmpty(_name) ? Id : _name!;
        init => _name = value;
    }

    /// <summary>
    /// Идентификатор пользователя в системе уведомлений
    /// </summary>
    public string? NotificationUserId { get; init; }

    /// <summary>
    /// Идентификатор команды/отдела
    /// </summary>
    public string? TeamId { get; init; }

    /// <summary>
    /// Идентификатор организации
    /// </summary>
    public string? OrganizationId { get; init; }

    /// <summary>
    /// Ссылка на фотографию пользователя
    /// </summary>
    public string? PhotoUrl { get; init; }

    /// <summary>
    /// Глубина пользователя в иерархии компании
    /// </summary>
    public int? Depth { get; init; }
}