using Api.Middleware;
using Microsoft.Extensions.DependencyInjection;

namespace Api;

public static class DiCompositor
{
    public static T AddApi<T>(this T serviceCollection)
        where T : IServiceCollection
    {
        serviceCollection.AddExceptionHandler<ExceptionHandlingMiddleware>();
        serviceCollection.AddProblemDetails();

        return serviceCollection;
    }
}
