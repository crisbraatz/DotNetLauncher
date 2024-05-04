using System.Diagnostics.Metrics;
using Domain;
using OpenTelemetry;
using OpenTelemetry.Metrics;

namespace Application.Observability;

public static class Meter
{
    private static System.Diagnostics.Metrics.Meter Instance { get; } =
        new(AppSettings.AppName, AppSettings.AppVersion);

    public static readonly Counter<int> UserEntityRequests = Instance.CreateCounter<int>("user_entity_requests");

    public static IOpenTelemetryBuilder AddMetrics(this IOpenTelemetryBuilder builder) =>
        builder.WithMetrics(x => x.AddAspNetCoreInstrumentation().AddMeter(Instance.Name));
}