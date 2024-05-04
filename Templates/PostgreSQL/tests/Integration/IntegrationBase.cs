using Infrastructure.PostgreSql;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Integration;

public abstract class IntegrationBase : IDisposable
{
    protected const string RequestedBy = "example@template.com";
    protected readonly CancellationToken Token = new();
    private readonly Lazy<DatabaseContext> _context;
    private DatabaseContext Context => _context.Value;
    private readonly Lazy<IServiceScope> _scope;

    protected IntegrationBase()
    {
        var services = BuildServiceCollection();

        var provider = new Lazy<IServiceProvider>(() =>
            new DefaultServiceProviderFactory(new ServiceProviderOptions()).CreateServiceProvider(services));

        _scope = new Lazy<IServiceScope>(() => provider.Value.CreateScope());

        _context = new Lazy<DatabaseContext>(GetRequiredService<DatabaseContext>);

        Context.Database.EnsureDeleted();
        Context.Database.EnsureCreated();
    }

    protected async Task CommitAsync() => await Context.SaveChangesAsync(Token);

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