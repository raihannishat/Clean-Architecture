using Microsoft.Extensions.DependencyInjection;
using BlogApp.API.Application.Features.Blog.Mapping;

namespace BlogApp.API.Application.Extensions
{
    public static class AutoMapperServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationAutoMapper(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(BlogMappingProfile));
            return services;
        }
    }
} 