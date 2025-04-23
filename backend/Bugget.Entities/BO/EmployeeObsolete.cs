namespace Bugget.Entities.BO;

public class EmployeeObsolete
{
    /// <summary>
    /// Уникальный идентификатор соответствующий хэдеру авторизации LDAP_USER_ID_KEY
    /// </summary>
    public required string Id { get; init; }
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public string? Surname { get; init; }
    
    /// <summary>
    /// Идентификатор пользователя в системе уведомлений
    /// </summary>
    public string? NotificationUserId { get; init; }
    
    /// <summary>
    /// Идентификатор команды/отдела
    /// </summary>
    public string? TeamId { get; init; }
    
    /// <summary>
    /// Название команды/отдела
    /// </summary>
    public string? TeamName { get; init; }
    
    /// <summary>
    /// Глубина пользователя в иерархии компании
    /// </summary>
    public int? Depth { get; init; }
}