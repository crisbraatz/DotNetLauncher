using Domain;
using Domain.Entities;
using Domain.Entities.Users;
using Infrastructure.MongoDb;
using Infrastructure.MongoDb.Repositories;
using Infrastructure.RabbitMq;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;

namespace Infrastructure;

public static class ServiceCollectionExtension
{
    public static void AddInfrastructure(this IServiceCollection services)
    {
        services.AddMongoDb();
        services.AddRabbitMq();
    }

    internal static void AddMongoDb(this IServiceCollection services)
    {
        services.AddSingleton(_ => new DatabaseHelper().Database);
        services.AddRepositories();
        services.AddHealthChecks().AddMongoDb(AppSettings.MongoDbConnectionString);
    }

    internal static void AddRabbitMq(this IServiceCollection services)
    {
        services.AddSingleton<IConnectionFactory>(_ => new ConnectionFactory
        {
            Uri = new Uri(AppSettings.RabbitMqConnectionString),
            DispatchConsumersAsync = true
        });
        services.AddSingleton<BasePublisher>();
        services.AddHealthChecks().AddRabbitMQ();
    }

    private static void AddRepositories(this IServiceCollection services) =>
        services.AddScoped<IBaseEntityRepository<User>, UsersRepository>();
}