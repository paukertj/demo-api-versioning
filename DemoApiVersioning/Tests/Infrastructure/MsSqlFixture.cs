using Infrastructure.Database;
using Testcontainers.MsSql;

namespace Tests.Infrastructure;

[TestClass]
public static class MsSqlFixture
{
    private static MsSqlContainer? _container;
    private static SettingsApiFactory? _factory;

    public static string ConnectionString =>
        _container?.GetConnectionString()
        ?? throw new InvalidOperationException("MS SQL container is not initialized.");

    public static SettingsApiFactory Factory =>
        _factory
        ?? throw new InvalidOperationException("API factory is not initialized.");

    [AssemblyInitialize]
    public static async Task InitializeAsync(TestContext _)
    {
        _container = new MsSqlBuilder().Build();
        await _container.StartAsync();

        _factory = new SettingsApiFactory();
        await DatabaseInitializer.MigrateAsync(_factory.Services);
    }

    [AssemblyCleanup]
    public static async Task CleanupAsync()
    {
        if (_factory is not null)
        {
            await _factory.DisposeAsync();
        }

        if (_container is not null)
        {
            await _container.DisposeAsync();
        }
    }
}
