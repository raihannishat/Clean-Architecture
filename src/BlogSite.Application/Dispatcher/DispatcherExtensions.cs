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

namespace BlogSite.Application.Dispatcher;

public static class DispatcherExtensions
{
    public static IServiceCollection AddDispatcher(this IServiceCollection services)
    {
        // Register dispatcher services
        services.AddSingleton<IRequestTypeRegistry, RequestTypeRegistry>();
        services.AddScoped<IDispatcher, Dispatcher>();

        return services;
    }

    public static IServiceProvider RegisterAllOperations(this IServiceProvider serviceProvider)
    {
        var registry = serviceProvider.GetRequiredService<IRequestTypeRegistry>();

        // Dynamic operation discovery and registration
        DiscoverAndRegisterOperations(registry);

        return serviceProvider;
    }

    private static void DiscoverAndRegisterOperations(IRequestTypeRegistry registry)
    {
        // Auto-discover all operations from assemblies
        var assemblies = new[]
        {
            Assembly.GetExecutingAssembly(), // Current assembly (Application)
            Assembly.GetCallingAssembly(),   // Assembly that called this
        };

        var operations = new List<OperationMetadata>();

        foreach (var assembly in assemblies)
        {
            // Find all IRequest implementations
            var requestTypes = assembly.GetTypes()
                .Where(type => IsValidRequestType(type))
                .ToList();

            foreach (var requestType in requestTypes)
            {
                var metadata = ParseTypeToOperationMetadata(requestType);
                if (metadata != null)
                {
                    operations.Add(metadata);
                }
            }
        }

        // Log discovered operations for debugging
        Console.WriteLine($"🔍 Discovered {operations.Count} operations:");
        foreach (var op in operations.OrderBy(o => o.EntityType).ThenBy(o => o.OperationType).ThenBy(o => o.Action))
        {
            Console.WriteLine($"   ✅ {op.OperationType} | {op.EntityType} | {op.Action} → {op.RequestType.Name}");
        }
    }

    private static bool IsValidRequestType(Type type)
    {
        if (type.IsAbstract || type.IsInterface || type.IsGenericTypeDefinition)
            return false;

        // Check if it implements IRequest or IRequest<T>
        return typeof(IRequest).IsAssignableFrom(type) || 
               type.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequest<>));
    }

    private static OperationMetadata? ParseTypeToOperationMetadata(Type type)
    {
        var typeName = type.Name;
        
        // Determine operation type and remaining name
        string operationType;
        string remainingName;
        
        if (typeName.EndsWith("Query", StringComparison.OrdinalIgnoreCase))
        {
            operationType = "Query";
            remainingName = typeName[..^5]; // Remove "Query"
        }
        else if (typeName.EndsWith("Command", StringComparison.OrdinalIgnoreCase))
        {
            operationType = "Command";
            remainingName = typeName[..^7]; // Remove "Command"
        }
        else
        {
            return null; // Not a recognized pattern
        }

        // Parse {Action}{Entity} from remainingName
        var entityMatch = FindEntityInTypeName(remainingName);
        if (entityMatch == null)
            return null;

        var action = remainingName[..^entityMatch.Length];
        if (string.IsNullOrEmpty(action))
            return null;

        // Get response type
        var responseType = GetResponseType(type);
        
        return new OperationMetadata(operationType, entityMatch, action, type, responseType);
    }

    private static string? FindEntityInTypeName(string typeName)
    {
        // Known entity patterns - could be made more dynamic by scanning domain entities
        var knownEntities = new[] { "Author", "BlogPost", "Category", "Comment" };
        
        return knownEntities.FirstOrDefault(entity => 
            typeName.EndsWith(entity, StringComparison.OrdinalIgnoreCase));
    }

    private static Type? GetResponseType(Type requestType)
    {
        // Check if it implements IRequest<T>
        var requestInterface = requestType.GetInterfaces()
            .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequest<>));

        return requestInterface?.GetGenericArguments().FirstOrDefault();
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