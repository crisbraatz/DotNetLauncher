using Application;
using Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace Integration;

public static class ServiceCollectionExtension
{
    public static void AddIntegration(this IServiceCollection services)
    {
        Environment.SetEnvironmentVariable("MONGODB_DATABASE", "integration-tests-TemplateMongoDB");

        services.AddMongoDb();
        services.AddRabbitMq();
        services.AddServices();
    }
}