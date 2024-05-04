using Application.Observability;
using Domain;
using Microsoft.AspNetCore.Builder;

namespace Application;

public static class ConfigureHostBuilderExtension
{
    public static void AddObservability(this ConfigureHostBuilder builder)
    {
        if (!AppSettings.IsDevelopment)
            builder.AddLogs();
    }
}