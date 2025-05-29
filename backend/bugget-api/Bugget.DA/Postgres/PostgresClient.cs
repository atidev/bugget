using System.Text.Json;
using Bugget.Entities.Constants;
using Npgsql;

namespace Bugget.DA.Postgres;

public abstract class PostgresClient
{
    protected readonly NpgsqlDataSource DataSource = NpgsqlDataSource.Create(Environment.GetEnvironmentVariable(EnvironmentConstants.PostgresConnectionString) 
                                                                            ?? throw new ApplicationException($"Не задана строка подключения к Postgres, env=[{EnvironmentConstants.PostgresConnectionString}]"));
    protected readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
    };
}