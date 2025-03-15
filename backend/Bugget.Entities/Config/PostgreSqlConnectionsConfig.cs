namespace Bugget.Entities.Config;

public class PostgresSqlConnectionsConfig
{
    /// <summary>
    /// Строки подключения
    /// </summary>
    public required Dictionary<string, string> DefaultConnections { get; set; }
}