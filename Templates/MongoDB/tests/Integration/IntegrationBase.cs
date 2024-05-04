using Infrastructure.MongoDb;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Integration;

public abstract class IntegrationBase : IDisposable
{
    protected const string RequestedBy = "example@template.com";
    protected readonly CancellationToken Token = new();
    private readonly Lazy<IMongoDatabase> _database;
    private IMongoDatabase Database => _database.Value;
    private readonly Lazy<IServiceScope> _scope;

    protected IntegrationBase()
    {
        var services = BuildServiceCollection();

        var provider = new Lazy<IServiceProvider>(() =>
            new DefaultServiceProviderFactory(new ServiceProviderOptions()).CreateServiceProvider(services));

        _scope = new Lazy<IServiceScope>(() => provider.Value.CreateScope());

        _database = new Lazy<IMongoDatabase>(new DatabaseHelper().Database);

        var cursor = Database.ListCollectionNames();
        cursor.MoveNextAsync();
        foreach (var collection in cursor.Current)
            Database.DropCollection(collection);
    }

    protected T GetRequiredService<T>() where T : notnull => _scope.Value.ServiceProvider.GetRequiredService<T>();

    private static ServiceCollection BuildServiceCollection()
    {
        var services = new ServiceCollection();
        services.AddIntegration();
        services.AddLogging(x => x.AddConsole());

        return services;
    }

    public void Dispose()
    {
        if (_scope.IsValueCreated)
            _scope.Value.Dispose();

        GC.SuppressFinalize(this);
    }
}