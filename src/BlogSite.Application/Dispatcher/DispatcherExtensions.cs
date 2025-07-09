using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using MediatR;
using BlogSite.Application.Services;
using BlogSite.Application.Configuration;

namespace BlogSite.Application.Dispatcher;

public static class DispatcherExtensions
{
    public static IServiceCollection AddDispatcher(this IServiceCollection services)
    {
        // Register pluralization and entity discovery services
        services.AddSingleton<IPluralizationService, PluralizationService>();
        services.AddSingleton<IEntityDiscoveryService, EntityDiscoveryService>();
        
        // Register pattern matching service
        services.AddSingleton<IPatternMatcher, PatternMatcher>();
        
        // Register operation description generator (requires configuration)
        services.AddSingleton<IOperationDescriptionGenerator, SmartOperationDescriptionGenerator>();
        
        // Register dispatcher services
        services.AddSingleton<IRequestTypeRegistry, RequestTypeRegistry>();
        services.AddScoped<IDispatcher, Dispatcher>();

        return services;
    }

    /// <summary>
    /// Automatically discovers and registers all operations from the current assemblies.
    /// No manual registration needed for new entities.
    /// </summary>
    public static IServiceProvider RegisterAllOperations(this IServiceProvider serviceProvider)
    {
        // The RequestTypeRegistry already automatically discovers operations through reflection
        // No manual registration is needed - this method is kept for backward compatibility
        // and can be used to warm up the cache if needed
        
        var registry = serviceProvider.GetRequiredService<IRequestTypeRegistry>();
        
        // Optionally warm up the cache by calling GetAllOperations
        _ = registry.GetAllOperations().ToList();
        
        return serviceProvider;
    }

    /// <summary>
    /// Gets dynamic operation summaries for all discovered operations
    /// </summary>
    public static IEnumerable<OperationSummary> GetOperationSummaries(this IRequestTypeRegistry registry, IOperationDescriptionGenerator descriptionGenerator)
    {
        return registry.GetAllOperations().Select(op => new OperationSummary(
            OperationType: op.OperationType,
            EntityType: op.EntityType,
            Action: op.Action,
            RequestTypeName: op.RequestType.Name,
            ResponseTypeName: op.ResponseType?.Name ?? "void",
            Description: descriptionGenerator.GetDescription(op.RequestType, op.OperationType, op.EntityType, op.Action)
        ));
    }

    /// <summary>
    /// Gets all available entity types by discovering them from existing operations
    /// </summary>
    public static IEnumerable<string> GetAvailableEntityTypes(this IRequestTypeRegistry registry)
    {
        return registry.GetAllOperations()
            .Select(op => op.EntityType)
            .Distinct()
            .OrderBy(x => x);
    }

    /// <summary>
    /// Gets all available actions for a specific entity type
    /// </summary>
    public static IEnumerable<string> GetAvailableActions(this IRequestTypeRegistry registry, string entityType)
    {
        return registry.GetAllOperations()
            .Where(op => op.EntityType.Equals(entityType, StringComparison.OrdinalIgnoreCase))
            .Select(op => op.Action)
            .Distinct()
            .OrderBy(x => x);
    }

    /// <summary>
    /// Gets all available operations grouped by entity type
    /// </summary>
    public static Dictionary<string, List<OperationSummary>> GetOperationsByEntity(this IRequestTypeRegistry registry, IOperationDescriptionGenerator descriptionGenerator)
    {
        return registry.GetOperationSummaries(descriptionGenerator)
            .GroupBy(op => op.EntityType)
            .ToDictionary(
                g => g.Key,
                g => g.OrderBy(op => op.OperationType).ThenBy(op => op.Action).ToList()
            );
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