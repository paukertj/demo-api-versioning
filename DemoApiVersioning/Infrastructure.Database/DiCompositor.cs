using Infrastructure.Database.Options;
using Infrastructure.Database.Repositories.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Infrastructure.Database;

public static class DiCompositor
{
    public static T AddDatabase<T>(this T serviceCollection)
        where T : IServiceCollection
    {
        serviceCollection
            .AddOptions<DatabaseOptions>()
            .BindConfiguration(DatabaseOptions.SectionName);

        serviceCollection.AddDbContext<DatabaseContext>((serviceProvider, options) =>
        {
            var databaseOptions = serviceProvider
                .GetRequiredService<IOptions<DatabaseOptions>>()
                .Value;

            options.UseSqlServer(databaseOptions.ConnectionString);
        });

        serviceCollection.AddScoped<ISettingsRepository, SettingsRepository>();

        return serviceCollection;
    }
}
