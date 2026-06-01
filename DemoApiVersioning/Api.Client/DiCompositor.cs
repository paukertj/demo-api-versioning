using Api.Abstractions.V1;
using Microsoft.Extensions.DependencyInjection;

namespace Api.Client;

public static class DiCompositor
{
    public static T AddClient<T>(this T serviceCollection, Action<HttpClient>? configureClient = null)
        where T : IServiceCollection
    {
        var builder = serviceCollection.AddHttpClient<ISettingsContractV1, SettingsClientServiceV1>();

        if (configureClient is not null)
        {
            builder.ConfigureHttpClient(configureClient);
        }

        return serviceCollection;
    }
}
