using Api.Abstractions.V1;
using Api.Abstractions.V2;
using Microsoft.Extensions.DependencyInjection;

namespace Api.Client;

public static class DiCompositor
{
    public static T AddClient<T>(this T serviceCollection, Action<HttpClient>? configureClient = null)
        where T : IServiceCollection
    {
        var v1Builder = serviceCollection.AddHttpClient<ISettingsContractV1, SettingsClientServiceV1>();
        var v2Builder = serviceCollection.AddHttpClient<ISettingsContractV2, SettingsClientServiceV2>();

        if (configureClient is not null)
        {
            v1Builder.ConfigureHttpClient(configureClient);
            v2Builder.ConfigureHttpClient(configureClient);
        }

        return serviceCollection;
    }
}
