namespace Bugget.Entities.BO;

public class Employee
{
    /// <summary>
    /// Уникальный идентификатор соответствующий хэдеру авторизации LDAP_USER_ID_KEY
    /// </summary>
    public required string Id { get; init; }

    /// <summary>
    /// Имя пользователя
    /// </summary>
    public string? Name { get; init; }
    
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