using SixLabors.ImageSharp;
using Serilog;
using Bugget.Extensions;
using Bugget.Logging;

public class Program
{
    public static async Task Main(string[] args)
    {
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
            await app.RunAsync();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Fatal host error");
        }
        finally
        {
            await Log.CloseAndFlushAsync();
        }
    }
}