using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using DbUp;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Bugget.Entities.Constants;

namespace Bugget.DbUp;

public sealed class DbUpService(ILogger<DbUpService> logger) : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        var connectionString = Environment.GetEnvironmentVariable(EnvironmentConstants.PostgresConnectionString) 
                                                                            ?? throw new ApplicationException($"Не задана строка подключения к Postgres, env=[{EnvironmentConstants.PostgresConnectionString}]");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            logger.LogError("No connection string provided.");
            return Task.CompletedTask;
        }

        var upgrader = DeployChanges.To
            .PostgresqlDatabase(connectionString)
            .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
            .WithTransaction()
            .LogToConsole()
            .Build();

        var result = upgrader.PerformUpgrade();

        if (!result.Successful)
        {
            logger.LogError(result.Error, "Migration failed");
        }
        else
        {
            logger.LogInformation("Database migration successful");
        }

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
