namespace BlogApp.API.Api.Configuration;

public static class FastEndpointsConfig
{
    public static IServiceCollection AddFastEndpointsConfig(this IServiceCollection services)
    {
        services.AddFastEndpoints();
        services.AddSwaggerDocument();
        return services;
    }
} 