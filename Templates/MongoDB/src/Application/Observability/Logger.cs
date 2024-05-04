using Domain;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Filters;

namespace Application.Observability;

public static class Logger
{
    public static void AddLogs(this IHostBuilder builder) => builder.UseSerilog((_, configuration) =>
    {
        configuration
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
            .Filter.ByExcluding(Matching.WithProperty<string>("RequestPath", x => x.Contains("healthcheck")))
            .Enrich.WithProperty("app_environment", AppSettings.AppEnvironment)
            .Enrich.WithProperty("app_name", AppSettings.AppName)
            .Enrich.WithProperty("app_version", AppSettings.AppVersion)
            .Enrich.FromLogContext()
            .WriteTo.Console();
    });
}