using FastEndpoints;
using FastEndpoints.Swagger;

namespace BlogApp.API.Configuration;

public static class FastEndpointsConfig
{
    public static IServiceCollection AddFastEndpointsConfig(this IServiceCollection services)
    {
        services.AddFastEndpoints();
        services.AddSwaggerDocument();
        return services;
    }
} 