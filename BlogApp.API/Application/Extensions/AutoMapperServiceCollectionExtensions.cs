namespace BlogApp.API.Application.Extensions
{
    public static class AutoMapperServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationAutoMapper(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(BlogApp.API.Application.Features.Blog.Mapping.BlogMappingProfile));
            return services;
        }
    }
} 