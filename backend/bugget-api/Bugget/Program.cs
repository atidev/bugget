using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using Bugget.Authentication;
using Bugget.BO.Services;
using Bugget.DA.Files;
using Bugget.DA.Postgres;
using Bugget.Hubs;
using Bugget.Middlewares;
using Bugget.DA.Interfaces;
using Bugget.ExternalClients;
using TaskQueue;
using Microsoft.AspNetCore.SignalR;
using Bugget.DA.WebSockets;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Bugget.Entities.Options;
using Bugget.BO.Services.Reports;
using Bugget.BO.Services.Comments;
using Bugget.BO.Services.Attachments;
using Bugget.BO.Interfaces;
using Bugget.Configurations;
using SixLabors.ImageSharp;
using OpenTelemetry.Metrics;
using Serilog;
using Bugget.Extensions;
using Bugget.Logging;

Log.Logger = BootstrapLogger.Create();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Configuration
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
        .AddEnvironmentVariables()
        .AddCommandLine(args);

    var externalPath = Path.Combine(AppContext.BaseDirectory, "external_settings.json");
    if (File.Exists(externalPath))
    {
        builder.Configuration.AddJsonFile(externalPath, optional: false, reloadOnChange: true);
    }
    else
    {
        Console.WriteLine("File external_settings.json not found");
    }

    builder.Services
        .AddConfiguration(builder.Configuration)
        .AddLogging(builder.Configuration)
        .AddDataAccess(builder.Configuration)
        .AddBusinessLogic()
        .AddMessaging()
        .AddWebApi(builder.Configuration, builder.Environment);

    #region заклинания
    Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
    Configuration.Default.MaxDegreeOfParallelism = Environment.ProcessorCount;
    Configuration.Default.PreferContiguousImageBuffers = true;
    #endregion

    var app = builder.Build();
    app.UsePipeline();
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Fatal host error");
}
finally
{
    await Log.CloseAndFlushAsync();
}