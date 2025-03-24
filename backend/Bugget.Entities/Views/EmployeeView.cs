namespace Bugget.Entities.Views;

public class EmployeeView
{
    /// <summary>
    /// Id пользователя
    /// </summary>
    public required string Id { get; set; }
    
    /// <summary>
    /// Полное имя пользователя
    /// </summary>
    public required string Name { get; set; }
}