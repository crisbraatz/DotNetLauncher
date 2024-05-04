using Application.Observability;
using Application.Services.Users;
using Domain;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Application;

public static class ServiceCollectionExtension
{
    public static void AddApplication(this IServiceCollection services) => services.AddServices();

    public static void AddObservability(this IServiceCollection services)
    {
        if (!AppSettings.IsDevelopment)
            services.AddOpenTelemetry().AddMetrics().AddTraces();
    }

    internal static void AddServices(this IServiceCollection services)
    {
        services.AddScoped<IUserAuthenticator, UserAuthenticator>();
        services.AddScoped<IUserCreator, UserCreator>();
        services.AddScoped<IUserDeactivator, UserDeactivator>();
        services.AddScoped<IUserList, UserList>();
        services.AddScoped<IUserPasswordUpdater, UserPasswordUpdater>();

        services.Configure<HostOptions>(x =>
        {
            x.ServicesStartConcurrently = true;
            x.ServicesStopConcurrently = true;
        });

        services.AddHostedService<UserMeterReceiver>();
    }
}