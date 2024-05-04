using System.Diagnostics;
using Domain;
using OpenTelemetry;
using OpenTelemetry.Trace;

namespace Application.Observability;

public static class Tracer
{
    public static ActivitySource Instance { get; } = new(AppSettings.AppName, AppSettings.AppVersion);

    public static void AddTraces(this IOpenTelemetryBuilder builder) => builder.WithTracing(x => x
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddSource(Instance.Name)
        .AddConsoleExporter());
}