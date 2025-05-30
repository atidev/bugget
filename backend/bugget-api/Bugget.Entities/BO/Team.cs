namespace Bugget.Entities.BO;

public class Team
{
    /// <summary>
    /// Идентификатор команды
    /// </summary>
    public required string Id { get; init; }
    
    /// <summary>
    /// Название команды
    /// </summary>
    public required string Name { get; init; }
    
    /// <summary>
    /// Глубина команды в иерархии компании
    /// </summary>
    public int? Depth { get; init; }
}