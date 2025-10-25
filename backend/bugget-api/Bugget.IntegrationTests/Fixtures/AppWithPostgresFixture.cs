using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Testcontainers.PostgreSql;

namespace Bugget.IntegrationTests.Fixtures;

public class AppWithPostgresFixture(PostgresContainerFixture fixture)
    : WebApplicationFactory<Program>
{
    private readonly PostgreSqlContainer _db = fixture.Container;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        Environment.SetEnvironmentVariable("POSTGRES_CONNECTION_STRING", _db.GetConnectionString());

        builder.ConfigureTestServices(services =>
        {
            // убираем все хостед сервисы в том числе и DbUp
            services.RemoveAll<IHostedService>();
        });
    }
}

