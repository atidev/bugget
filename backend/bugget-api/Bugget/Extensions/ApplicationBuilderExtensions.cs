using Bugget.Configurations;
using Bugget.Middlewares;
using Bugget.Hubs;
using Serilog;

namespace Bugget.Extensions;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UsePipeline(this IApplicationBuilder app)
    {
        app.UseSwaggerConfiguration();
        app.UseSerilogRequestLogging();
        app.UseMiddleware<ResultExceptionHandlerMiddleware>();
        app.UseCors("CorsPolicy");
        app.UseRouting();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapHub<ReportPageHub>("/v1/report-page-hub");
            endpoints.MapHealthChecks("/_internal/ping");
        });

        app.UseOpenTelemetryPrometheusScrapingEndpoint();

        return app;
    }
} 