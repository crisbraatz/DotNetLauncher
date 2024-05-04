using Application;
using Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace Integration;

public static class ServiceCollectionExtension
{
    public static void AddIntegration(this IServiceCollection services)
    {
        Environment.SetEnvironmentVariable("POSTGRESQL_CONNECTION_STRING",
            "Host=127.0.0.1;Port=5432;Database=integration-tests-TemplatePostgreSQL;Username=postgres;Password=postgres;Command Timeout=60;Timeout=60;SSL Mode=Disable;Trust Server Certificate=True");

        services.AddPostgreSql();
        services.AddRabbitMq();
        services.AddServices();
    }
}