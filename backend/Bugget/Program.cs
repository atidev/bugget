using System.Text.Json;
using System.Text.RegularExpressions;
using Bugget.Authentication;
using Bugget.BO.Services;
using Bugget.DA.Files;
using Bugget.DA.Postgres;
using Bugget.Entities.Config;
using Bugget.Features;
using Bugget.Features.TaskQueue;
using Bugget.Hubs;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables()
    .AddCommandLine(args);

builder.Services.AddSignalR(options =>
{
    options.EnableDetailedErrors = true; // Показывать ошибки в логе
    options.KeepAliveInterval = TimeSpan.FromSeconds(15); // Пинг каждые 15 сек
    options.ClientTimeoutInterval = TimeSpan.FromSeconds(60); // Клиент ждёт 60 сек перед разрывом
});

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


builder.Services.Configure<PostgresSqlConnectionsConfig>(builder.Configuration.GetSection(nameof(PostgresSqlConnectionsConfig)));
builder.Services.Configure<FileStorageConfiguration>(
    builder.Configuration.GetSection(nameof(FileStorageConfiguration)));

builder.Services.AddControllers()
    .AddJsonOptions(options => { options.JsonSerializerOptions.PropertyNamingPolicy = SnakeCaseNamingPolicy.Instance; });

builder.Services.AddAutoMapper(typeof(Bugget.Entities.MappingProfiles.BugMappingProfile).Assembly);

builder.Services.AddFeatures();

builder.Services
    .AddSingleton<ReportsService>()
    .AddSingleton<BugsService>()
    .AddSingleton<CommentsService>()
    .AddSingleton<EmployeesService>()
    .AddSingleton<AttachmentService>();

builder.Services
    .AddSingleton<ReportsDbClient>()
    .AddSingleton<CommentsDbClient>()
    .AddSingleton<BugsDbClient>()
    .AddSingleton<AttachmentDbClient>()
    .AddSingleton<EmployeesDataAccess>();

builder.Services.AddHostedService((sp) => sp.GetRequiredService<EmployeesDataAccess>());

builder.Services.AddHealthChecks();
builder.Services.AddLdapAuth();
builder.Services.AddTaskQueueHostedService();

#region Swagger

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Report API", Version = "v1" });
    // Подключаем XML-документацию
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});

#endregion



var app = builder.Build();

#region Swagger

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Report API v1");
    c.RoutePrefix = ""; // Доступ по http://localhost:7777/swagger
});

#endregion

app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/_ping");


app.UseCors("CorsPolicy");


app.MapHub<ReportPageHub>("/bugget/public/v1/report-page-hub"); // Подключаем хаб

app.Run();


public class SnakeCaseNamingPolicy : JsonNamingPolicy
{
    public static readonly SnakeCaseNamingPolicy Instance = new();

    public override string ConvertName(string name)
    {
        return Regex.Replace(name, "([a-z0-9])([A-Z])", "$1_$2").ToLower();
    }
}