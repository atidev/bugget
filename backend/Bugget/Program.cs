using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using Bugget.Authentication;
using Bugget.BO.Services;
using Bugget.DA.Files;
using Bugget.DA.Postgres;
using Bugget.Hubs;
using Bugget.Middlewares;
using Microsoft.OpenApi.Models;
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

builder.Services.AddSingleton<IUserIdProvider, SignalRUserIdProvider>();
builder.Services.AddSignalR(options =>
    {
        options.EnableDetailedErrors = true; // Показывать ошибки в логе
        options.KeepAliveInterval = TimeSpan.FromSeconds(15); // Пинг каждые 15 сек
        options.ClientTimeoutInterval = TimeSpan.FromSeconds(60); // Клиент ждёт 60 сек перед разрывом
    })
    .AddJsonProtocol(options =>
    {
        options.PayloadSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    })
    .AddHubOptions<ReportPageHub>(options => { options.AddFilter<HubExceptionHandlerFilter>(); });

// разрешаем cors для локального тестирования
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy =>
    {
        policy.SetIsOriginAllowed(_ => true) // Разрешает все Origins, но с `AllowCredentials()`
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials(); // Разрешаем куки и WebSockets
    });
});

builder.Services.Configure<FileStorageOptions>(
    builder.Configuration.GetSection(nameof(FileStorageOptions)));
builder.Services.Configure<AuthHeadersOptions>(
    builder.Configuration.GetSection("ExternalSettings:Authentication"));

builder.Services.AddAutoMapper(typeof(Bugget.Entities.MappingProfiles.BugMappingProfile).Assembly);

builder.Services.AddExternalClients();

builder.Services
    .AddSingleton<ReportsService>()
    .AddSingleton<BugsService>()
    .AddSingleton<BugsEventsService>()
    .AddSingleton<AttachmentObsoleteService>()
    .AddSingleton<ReportEventsService>()
    .AddSingleton<ReportAutoStatusService>()
    .AddSingleton<ParticipantsService>()
    .AddSingleton<AttachmentOptimizator>()
    .AddSingleton<AttachmentService>()
    .AddSingleton<AttachmentEventsService>()
    .AddSingleton<ImageOptimizeWriter>()
    .AddSingleton<TextOptimizeWriter>()
    .AddSingleton<AttachmentOptimizator>()
    .AddSingleton<IAttachmentKeyGenerator,LocalAttachmentKeyGenerator>()
    .AddSingleton<CommentsObsoleteService>()
    .AddSingleton<CommentsService>()
    .AddSingleton<CommentEventsService>()
    .AddSingleton<LimitsService>();

builder.Services
    .AddSingleton<ReportsDbClient>()
    .AddSingleton<CommentsObsoleteDbClient>()
    .AddSingleton<CommentsDbClient>()
    .AddSingleton<BugsDbClient>()
    .AddSingleton<AttachmentObsoleteDbClient>()
    .AddSingleton<AttachmentDbClient>()
    .AddSingleton<EmployeesFileClient>()
    .AddSingleton<TeamsFileClient>()
    .AddSingleton<ParticipantsDbClient>()
    .AddSingleton<IEmployeesClient>((sp) => sp.GetRequiredService<EmployeesFileClient>())
    .AddSingleton<ITeamsClient>((sp) => sp.GetRequiredService<TeamsFileClient>())
    .AddSingleton<IFileStorageClient,LocalFileStorageClient>();

builder.Services.AddHostedService((sp) => sp.GetRequiredService<EmployeesFileClient>());
builder.Services.AddHostedService((sp) => sp.GetRequiredService<TeamsFileClient>());

builder.Services.AddHealthChecks();
builder.Services.AddAuthHeaders();
builder.Services.AddSingleton<ITaskQueue, TaskQueue.TaskQueue>()
    .AddHostedService(provider => (TaskQueue.TaskQueue)provider.GetRequiredService<ITaskQueue>());

builder.Services.AddSwaggerConfiguration(builder.Configuration);

builder.Services.AddSingleton<ResultExceptionHandlerMiddleware>();

builder.Services.AddControllers((options) =>
{
    var policy = new AuthorizationPolicyBuilder()
        .AddAuthenticationSchemes(AuthSchemeNames.Headers)
        .RequireAuthenticatedUser()
        .Build();

    options.Filters.Add(new AuthorizeFilter(policy));
})
.AddJsonOptions(options => { options.JsonSerializerOptions.PropertyNamingPolicy = SnakeCaseNamingPolicy.Instance; })
    .ConfigureApiBehaviorOptions(o =>
        o.InvalidModelStateResponseFactory = _ => new ModelStateInvalidHandler());

builder.Services.AddSingleton<IReportPageHubClient, ReportPageHubClient>();

#region заклинания
// для маппинга snake_case в c# типы средствами dapper
Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
// настройка image sharp
Configuration.Default.MaxDegreeOfParallelism = Environment.ProcessorCount; // multi-threaded encode
Configuration.Default.PreferContiguousImageBuffers = true; // меньше аллокаций в обработке
#endregion

var app = builder.Build();

app.UseSwaggerConfiguration();

// регистрируем до авторизации
app.MapHealthChecks("/_internal/ping");

app.MapControllers();

app.UseCors("CorsPolicy");

app.UseMiddleware<ResultExceptionHandlerMiddleware>();

app.MapHub<ReportPageHub>("/v1/report-page-hub"); // Подключаем хаб

app.Run();


public class SnakeCaseNamingPolicy : JsonNamingPolicy
{
    public static readonly SnakeCaseNamingPolicy Instance = new();

    public override string ConvertName(string name)
    {
        return Regex.Replace(name, "([a-z0-9])([A-Z])", "$1_$2").ToLower();
    }
}