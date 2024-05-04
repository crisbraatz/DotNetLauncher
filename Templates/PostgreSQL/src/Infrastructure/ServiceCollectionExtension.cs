using Domain;
using Domain.Entities;
using Infrastructure.PostgreSql;
using Infrastructure.PostgreSql.Repositories;
using Infrastructure.RabbitMq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;

namespace Infrastructure;

public static class ServiceCollectionExtension
{
    public static void AddInfrastructure(this IServiceCollection services)
    {
        services.AddPostgreSql();
        services.AddRabbitMq();
    }

    internal static void AddPostgreSql(this IServiceCollection services)
    {
        services.AddDbContextPool<DatabaseContext>(x =>
            x.UseNpgsql(AppSettings.PostgreSqlConnectionString, y =>
            {
                y.MigrationsAssembly("Infrastructure");
                y.EnableRetryOnFailure();
            }));
        services.AddScoped(typeof(IBaseEntityRepository<>), typeof(BaseEntityRepository<>));
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        if (!AppSettings.IsDevelopment)
            services.BuildServiceProvider().GetRequiredService<DatabaseContext>().Database.Migrate();

        services.AddHealthChecks()
            .AddDbContextCheck<DatabaseContext>()
            .AddNpgSql(AppSettings.PostgreSqlConnectionString);
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
}