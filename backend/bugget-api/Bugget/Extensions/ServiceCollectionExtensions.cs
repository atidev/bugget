using Serilog;
using Bugget.Authentication;
using Bugget.DA.Interfaces;
using Bugget.DA.Files;
using Bugget.DA.Postgres;
using Bugget.BO.Services;
using Bugget.BO.Interfaces;
using Bugget.ExternalClients;
using TaskQueue;
using Bugget.DA.WebSockets;
using Bugget.Entities.Options;
using Bugget.Configurations;
using System.Text.Json.Serialization;
using System.Text.Json;
using Bugget.Hubs;
using Bugget.Middlewares;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Bugget.BO.Services.Reports;
using Bugget.BO.Services.Comments;
using Bugget.BO.Services.Attachments;
using OpenTelemetry.Metrics;
using Bugget.DbUp;

namespace Bugget.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<FileStorageOptions>(configuration.GetSection(nameof(FileStorageOptions)));
        services.Configure<AuthHeadersOptions>(configuration.GetSection("ExternalSettings:Authentication"));
        
        return services;
    }

    public static IServiceCollection AddLogging(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSerilog((ctx, lc) => lc.ReadFrom.Configuration(configuration));
        return services;
    }

    public static IServiceCollection AddDataAccess(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment env)
    {
        services
            .AddSingleton<ReportsDbClient>()
            .AddSingleton<CommentsDbClient>()
            .AddSingleton<BugsDbClient>()
            .AddSingleton<AttachmentDbClient>()
            .AddSingleton<UsersFileClient>()
            .AddSingleton<TeamsFileClient>()
            .AddSingleton<ParticipantsDbClient>()
            .AddSingleton<IUsersClient>((sp) => sp.GetRequiredService<UsersFileClient>())
            .AddSingleton<ITeamsClient>((sp) => sp.GetRequiredService<TeamsFileClient>())
            .AddSingleton<IFileStorageClient, LocalFileStorageClient>();

        services.AddHostedService((sp) => sp.GetRequiredService<UsersFileClient>());
        services.AddHostedService((sp) => sp.GetRequiredService<TeamsFileClient>());

        if(!env.IsDevelopment())
        {
            services.AddHostedService<DbUpService>();
        }

        return services;
    }

    public static IServiceCollection AddBusinessLogic(this IServiceCollection services)
    {
        services
            .AddSingleton<ReportsService>()
            .AddSingleton<BugsService>()
            .AddSingleton<BugsEventsService>()
            .AddSingleton<ReportEventsService>()
            .AddSingleton<ReportAutoStatusService>()
            .AddSingleton<ParticipantsService>()
            .AddSingleton<AttachmentOptimizator>()
            .AddSingleton<AttachmentService>()
            .AddSingleton<AttachmentEventsService>()
            .AddSingleton<ImageOptimizeWriter>()
            .AddSingleton<TextOptimizeWriter>()
            .AddSingleton<IAttachmentKeyGenerator, LocalAttachmentKeyGenerator>()
            .AddSingleton<CommentsService>()
            .AddSingleton<CommentEventsService>()
            .AddSingleton<LimitsService>();

        return services;
    }

    public static IServiceCollection AddMessaging(this IServiceCollection services)
    {
        services.AddSingleton<IUserIdProvider, SignalRUserIdProvider>();
        services.AddSignalR(options =>
        {
            options.EnableDetailedErrors = true;
            options.KeepAliveInterval = TimeSpan.FromSeconds(15);
            options.ClientTimeoutInterval = TimeSpan.FromSeconds(60);
        })
        .AddJsonProtocol(options =>
        {
            options.PayloadSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        })
        .AddHubOptions<ReportPageHub>(options => { options.AddFilter<HubExceptionHandlerFilter>(); });

        services.AddSingleton<ITaskQueue, TaskQueue.TaskQueue>()
            .AddHostedService(provider => (TaskQueue.TaskQueue)provider.GetRequiredService<ITaskQueue>());

        return services;
    }

    public static IServiceCollection AddWebApi(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment env)
    {
        services.AddExternalClients();
        services.AddHealthChecks();
        services.AddAuthHeaders();
        services.AddSwaggerConfiguration(configuration);
        services.AddSingleton<ResultExceptionHandlerMiddleware>();

        services.AddControllers(options =>
        {
            var policy = new AuthorizationPolicyBuilder()
                .AddAuthenticationSchemes(AuthSchemeNames.Headers)
                .RequireAuthenticatedUser()
                .Build();

            options.Filters.Add(new AuthorizeFilter(policy));
        })
        .AddJsonOptions(options => { options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower; })
        .ConfigureApiBehaviorOptions(o => o.InvalidModelStateResponseFactory = _ => new ModelStateInvalidHandler());

        services.AddSingleton<IReportPageHubClient, ReportPageHubClient>();

        services.AddOpenTelemetry()
            .WithMetrics(metrics => metrics
                .AddRuntimeInstrumentation()
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddPrometheusExporter());

        services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy", policy =>
            {
                policy.SetIsOriginAllowed(_ => true)
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials();
            });
        });

        return services;
    }
} 