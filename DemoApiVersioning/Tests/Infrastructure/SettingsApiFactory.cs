using Api;
using Api.Abstractions.V1;
using Api.Client;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Tests.Infrastructure;

public sealed class SettingsApiFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Database:ConnectionString"] = MsSqlFixture.ConnectionString,
            });
        });

        builder.ConfigureTestServices(services =>
        {
            services.AddClient(c => c.BaseAddress = new Uri("http://localhost"));

            services.AddHttpClient<ISettingsContractV1, SettingsClientServiceV1>()
                .ConfigurePrimaryHttpMessageHandler(() => Server.CreateHandler());
        });
    }
}
