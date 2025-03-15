namespace Bugget.Entities.Views;

public class EmployeeView
{
    /// <summary>
    /// Id пользователя
    /// </summary>
    public required string UserId { get; set; }
    
    /// <summary>
    /// Полное имя пользователя
    /// </summary>
    public required string FullName { get; set; }
}