using Core.Abstractions.Services;
using Core.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Core;

public static class DiCompositor
{
    public static T AddCore<T>(this T serviceCollection)
        where T : IServiceCollection
    {
        serviceCollection.AddScoped<ISettingsService, SettingsService>();
        
        return serviceCollection;
    }
}
