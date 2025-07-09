using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using MediatR;
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
    public static Dictionary<string, List<OperationSummary>> GetOperationsByEntity(this IRequestTypeRegistry registry)
    {
        return registry.GetOperationSummaries()
            .GroupBy(op => op.EntityType)
            .ToDictionary(
                g => g.Key,
                g => g.OrderBy(op => op.OperationType).ThenBy(op => op.Action).ToList()
            );
    }

    /// <summary>
    /// Generates dynamic operation descriptions based on naming conventions
    /// </summary>
    private static string GetOperationDescription(string operationType, string entityType, string action)
    {
        var entityLower = entityType.ToLower();
        var actionLower = action.ToLower();

        return operationType.ToLower() switch
        {
            "command" => actionLower switch
            {
                "create" => $"Creates a new {entityLower}",
                "update" => $"Updates an existing {entityLower}",
                "delete" => $"Deletes a {entityLower}",
                "publish" => $"Publishes a {entityLower}",
                "archive" => $"Archives a {entityLower}",
                "activate" => $"Activates a {entityLower}",
                "deactivate" => $"Deactivates a {entityLower}",
                "approve" => $"Approves a {entityLower}",
                "reject" => $"Rejects a {entityLower}",
                "enable" => $"Enables a {entityLower}",
                "disable" => $"Disables a {entityLower}",
                _ => $"Executes {action} command on {entityLower}"
            },
            "query" => actionLower switch
            {
                "getall" => $"Gets all {entityLower}s",
                "getbyid" => $"Gets a {entityLower} by ID",
                "getbyemail" => $"Gets a {entityLower} by email",
                "getbyname" => $"Gets a {entityLower} by name",
                "getbytitle" => $"Gets a {entityLower} by title",
                "getbyslug" => $"Gets a {entityLower} by slug",
                "getpublished" => $"Gets published {entityLower}s",
                "getactive" => $"Gets active {entityLower}s",
                "getarchived" => $"Gets archived {entityLower}s",
                "getbycategory" => $"Gets {entityLower}s by category",
                "getbyauthor" => $"Gets {entityLower}s by author",
                "getbyuser" => $"Gets {entityLower}s by user",
                "getbytag" => $"Gets {entityLower}s by tag",
                "search" => $"Searches {entityLower}s",
                _ => $"Executes {action} query on {entityLower}"
            },
            _ => $"Executes {operationType} {action} on {entityLower}"
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