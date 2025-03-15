using System.Text.Json;
using Bugget.Entities.Constants;

namespace Bugget.DA.Postgres;

public abstract class PostgresClient
{
    protected readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
    };
    
    protected static readonly string ConnectionString = 
        Environment.GetEnvironmentVariable(EnvironmentConstants.PostgresConnectionString) 
        ?? throw new ApplicationException($"Не задана строка подключения к Postgres, env=[{EnvironmentConstants.PostgresConnectionString}]");

}