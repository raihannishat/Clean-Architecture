using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using MediatR;
using BlogSite.Application.Commands.Authors;
using BlogSite.Application.Commands.BlogPosts;
using BlogSite.Application.Commands.Categories;
using BlogSite.Application.Queries.Authors;
using BlogSite.Application.Queries.BlogPosts;
using BlogSite.Application.Queries.Categories;
using BlogSite.Application.DTOs;
using BlogSite.Application.Services;

namespace BlogSite.Application.Dispatcher;

public static class DispatcherExtensions
{
    public static IServiceCollection AddDispatcher(this IServiceCollection services)
    {
        // Register pluralization and entity discovery services
        services.AddSingleton<IPluralizationService, PluralizationService>();
        services.AddSingleton<IEntityDiscoveryService, EntityDiscoveryService>();
        
        // Register dispatcher services
        services.AddSingleton<IRequestTypeRegistry, RequestTypeRegistry>();
        services.AddScoped<IDispatcher, Dispatcher>();

        return services;
    }

    public static IServiceProvider RegisterAllOperations(this IServiceProvider serviceProvider)
    {
        var registry = serviceProvider.GetRequiredService<IRequestTypeRegistry>();

        // Register Author operations
        RegisterAuthorOperations(registry);

        // Register BlogPost operations
        RegisterBlogPostOperations(registry);

        // Register Category operations
        RegisterCategoryOperations(registry);

        return serviceProvider;
    }

    private static void RegisterAuthorOperations(IRequestTypeRegistry registry)
    {
        // Author Commands
        registry.RegisterOperation("Command", "Author", "Create", typeof(CreateAuthorCommand), typeof(AuthorDto));
        registry.RegisterOperation("Command", "Author", "Update", typeof(UpdateAuthorCommand), typeof(AuthorDto));
        registry.RegisterOperation("Command", "Author", "Delete", typeof(DeleteAuthorCommand), typeof(bool));

        // Author Queries
        registry.RegisterOperation("Query", "Author", "GetAll", typeof(GetAllAuthorsQuery), typeof(IEnumerable<AuthorDto>));
        registry.RegisterOperation("Query", "Author", "GetById", typeof(GetAuthorByIdQuery), typeof(AuthorDto));
        registry.RegisterOperation("Query", "Author", "GetByEmail", typeof(GetAuthorByEmailQuery), typeof(AuthorDto));
    }

    private static void RegisterBlogPostOperations(IRequestTypeRegistry registry)
    {
        // BlogPost Commands
        registry.RegisterOperation("Command", "BlogPost", "Create", typeof(CreateBlogPostCommand), typeof(BlogPostDto));
        registry.RegisterOperation("Command", "BlogPost", "Publish", typeof(PublishBlogPostCommand), typeof(BlogPostDto));

        // BlogPost Queries
        registry.RegisterOperation("Query", "BlogPost", "GetPublished", typeof(GetPublishedBlogPostsQuery), typeof(IEnumerable<BlogPostDto>));
        registry.RegisterOperation("Query", "BlogPost", "GetByCategory", typeof(GetBlogPostsByCategoryQuery), typeof(IEnumerable<BlogPostDto>));
    }

    private static void RegisterCategoryOperations(IRequestTypeRegistry registry)
    {
        // Category Commands
        registry.RegisterOperation("Command", "Category", "Create", typeof(CreateCategoryCommand), typeof(CategoryDto));

        // Category Queries
        registry.RegisterOperation("Query", "Category", "GetAll", typeof(GetAllCategoriesQuery), typeof(IEnumerable<CategoryDto>));
    }

    public static IEnumerable<OperationSummary> GetOperationSummaries(this IRequestTypeRegistry registry)
    {
        return registry.GetAllOperations().Select(op => new OperationSummary(
            OperationType: op.OperationType,
            EntityType: op.EntityType,
            Action: op.Action,
            RequestTypeName: op.RequestType.Name,
            ResponseTypeName: op.ResponseType?.Name ?? "void",
            Description: GetOperationDescription(op.OperationType, op.EntityType, op.Action)
        ));
    }

    private static string GetOperationDescription(string operationType, string entityType, string action)
    {
        return operationType.ToLower() switch
        {
            "command" => action.ToLower() switch
            {
                "create" => $"Creates a new {entityType.ToLower()}",
                "update" => $"Updates an existing {entityType.ToLower()}",
                "delete" => $"Deletes a {entityType.ToLower()}",
                "publish" => $"Publishes a {entityType.ToLower()}",
                _ => $"Executes {action} command on {entityType.ToLower()}"
            },
            "query" => action.ToLower() switch
            {
                "getall" => $"Gets all {entityType.ToLower()}s",
                "getbyid" => $"Gets a {entityType.ToLower()} by ID",
                "getbyemail" => $"Gets a {entityType.ToLower()} by email",
                "getpublished" => $"Gets published {entityType.ToLower()}s",
                "getbycategory" => $"Gets {entityType.ToLower()}s by category",
                _ => $"Executes {action} query on {entityType.ToLower()}"
            },
            _ => $"Executes {operationType} {action} on {entityType}"
        };
    }
}

public record OperationSummary(
    string OperationType,
    string EntityType,
    string Action,
    string RequestTypeName,
    string ResponseTypeName,
    string Description
);