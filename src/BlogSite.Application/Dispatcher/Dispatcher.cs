using System.Text.Json;
using MediatR;
using System.Text.RegularExpressions;
using Microsoft.Extensions.DependencyInjection;
using BlogSite.Application.Services;

namespace BlogSite.Application.Dispatcher;

public class Dispatcher : IDispatcher
{
    private readonly IServiceProvider _provider;
    private readonly IRequestTypeRegistry _registry;
    private readonly IEntityDiscoveryService _entityDiscoveryService;
    private readonly IPluralizationService _pluralizationService;

    public Dispatcher(IServiceProvider provider, IRequestTypeRegistry registry, IEntityDiscoveryService entityDiscoveryService, IPluralizationService pluralizationService)
    {
        _provider = provider;
        _registry = registry;
        _entityDiscoveryService = entityDiscoveryService;
        _pluralizationService = pluralizationService;
    }

    public async Task<DispatchResult> DispatchAsync(DispatchRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await DispatchInternalAsync(request.Action, request.Payload ?? JsonDocument.Parse("{}").RootElement);
            return new DispatchResult(true, result);
        }
        catch (Exception ex)
        {
            return new DispatchResult(false, null, ex.Message, "DispatchError");
        }
    }

    private async Task<object?> DispatchInternalAsync(string action, JsonElement payload)
    {
        // Normalize action to lowercase for comparison
        var normalizedAction = action.ToLowerInvariant();
        
        // Determine operation type based on action prefix
        bool isQuery = normalizedAction.StartsWith("get");
        string suffix = isQuery ? "Query" : "Command";

        // Parse action dynamically to get class name
        string className = ParseActionToClassName(normalizedAction, suffix);

        var type = _registry.ResolveRequestType(className);
        if (type is null)
        {
            var availableActions = GetAvailableActions();
            throw new Exception($"Handler type '{className}' not found. Available actions: {string.Join(", ", availableActions)}");
        }

        var instance = payload.Deserialize(type)!;

        var resultType = type.GetInterfaces()
            .First(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequest<>))
            .GetGenericArguments()[0];

        var handlerType = typeof(IRequestHandler<,>).MakeGenericType(type, resultType);

        dynamic handler = _provider.GetRequiredService(handlerType);
        return await handler.Handle((dynamic)instance, CancellationToken.None);
    }

    private string ParseActionToClassName(string normalizedAction, string suffix)
    {
        // Handle "get by entity" patterns (e.g., "getbyauthor" -> "GetByAuthor")
        if (normalizedAction.StartsWith("getby"))
        {
            var entityPart = normalizedAction.Substring(5); // Remove "getby"
            return HandleGetByEntityPattern(entityPart, suffix);
        }

        // Handle "get entity by field" patterns (e.g., "getauthorbyid", "getauthorbyemail")
        if (normalizedAction.StartsWith("get") && normalizedAction.Contains("by"))
        {
            return HandleGetEntityByFieldPattern(normalizedAction, suffix);
        }

        // Handle "get all entities" patterns (e.g., "getallauthors", "getallcategories")
        if (normalizedAction.StartsWith("getall"))
        {
            var entityPart = normalizedAction.Substring(6); // Remove "getall"
            var entityName = GetEntityNameFromPlural(entityPart);
            return $"GetAll{ToPascalCase(entityName)}{suffix}";
        }

        // Handle "get published/specific entity" patterns (e.g., "getpublishedblogposts")
        if (normalizedAction.StartsWith("get") && !normalizedAction.Contains("by"))
        {
            return HandleGetSpecificPattern(normalizedAction, suffix);
        }

        // Handle command patterns (create, update, delete, publish)
        if (normalizedAction.StartsWith("create") || 
            normalizedAction.StartsWith("update") || 
            normalizedAction.StartsWith("delete") ||
            normalizedAction.StartsWith("publish"))
        {
            return HandleCommandPattern(normalizedAction, suffix);
        }

        // Dynamic parsing with smart entity detection
        var knownEntities = GetKnownEntities();
        
        // Try to parse patterns with entity recognition
        var bestMatch = FindBestEntityMatch(normalizedAction, knownEntities);
        
        if (bestMatch != null)
        {
            var actionPart = normalizedAction.Replace(bestMatch.EntityName.ToLowerInvariant(), "");
            var cleanAction = ToPascalCase(actionPart);
            return $"{cleanAction}{bestMatch.EntityName}{suffix}";
        }
        
        // Fallback: convert entire action to PascalCase
        return $"{ToPascalCase(normalizedAction)}{suffix}";
    }

    private EntityMatch? FindBestEntityMatch(string normalizedAction, string[] entities)
    {
        EntityMatch? bestMatch = null;
        int bestScore = 0;

        foreach (var entity in entities)
        {
            var entityLower = entity.ToLowerInvariant();
            
            // Check if action contains this entity name
            if (normalizedAction.Contains(entityLower))
            {
                // Score based on length and position
                int score = entityLower.Length;
                if (normalizedAction.EndsWith(entityLower))
                    score += 10; // Prefer entity at the end
                
                if (score > bestScore)
                {
                    bestScore = score;
                    bestMatch = new EntityMatch(entity, entityLower);
                }
            }
        }

        return bestMatch;
    }

    private string HandleGetByEntityPattern(string entityPart, string suffix)
    {
        // For patterns like "getbyauthor" -> "GetByAuthorQuery"
        var entityName = GetEntityNameFromSingularOrPlural(entityPart);
        return $"GetBy{ToPascalCase(entityName)}{suffix}";
    }

    private string HandleGetEntityByFieldPattern(string normalizedAction, string suffix)
    {
        // For patterns like "getauthorbyid" -> "GetAuthorById"
        var parts = normalizedAction.Split("by", 2);
        if (parts.Length == 2)
        {
            var entityPart = parts[0].Substring(3); // Remove "get"
            var fieldPart = parts[1];
            
            var entityName = GetEntityNameFromSingularOrPlural(entityPart);
            var fieldName = ToPascalCase(fieldPart);
            
            return $"Get{ToPascalCase(entityName)}By{fieldName}{suffix}";
        }

        return $"{ToPascalCase(normalizedAction)}{suffix}";
    }

    private string HandleGetSpecificPattern(string normalizedAction, string suffix)
    {
        // For patterns like "getpublishedblogposts" -> "GetPublishedBlogPosts"
        var actionPart = normalizedAction.Substring(3); // Remove "get"
        
        // Check for known patterns dynamically
        if (actionPart.Contains("published"))
        {
            var entityPart = actionPart.Replace("published", "");
            var entityName = GetEntityNameFromPlural(entityPart);
            return $"GetPublished{ToPascalCase(entityName)}{suffix}";
        }

        // Handle other modifiers dynamically
        var modifierPatterns = new[] { "active", "inactive", "featured", "recent", "popular", "archived" };
        foreach (var modifier in modifierPatterns)
        {
            if (actionPart.Contains(modifier))
            {
                var entityPart = actionPart.Replace(modifier, "");
                var entityName = GetEntityNameFromPlural(entityPart);
                return $"Get{ToPascalCase(modifier)}{ToPascalCase(entityName)}{suffix}";
            }
        }

        // Default handling - try to parse as compound words
        var parsed = ParseCamelCaseWords(actionPart);
        return $"Get{parsed}{suffix}";
    }

    private string HandleCommandPattern(string normalizedAction, string suffix)
    {
        // For patterns like "createauthor" -> "CreateAuthor", "publishblogpost" -> "PublishBlogPost"
        string command = "";
        string entityPart = "";

        if (normalizedAction.StartsWith("create"))
        {
            command = "Create";
            entityPart = normalizedAction.Substring(6);
        }
        else if (normalizedAction.StartsWith("update"))
        {
            command = "Update";
            entityPart = normalizedAction.Substring(6);
        }
        else if (normalizedAction.StartsWith("delete"))
        {
            command = "Delete";
            entityPart = normalizedAction.Substring(6);
        }
        else if (normalizedAction.StartsWith("publish"))
        {
            command = "Publish";
            entityPart = normalizedAction.Substring(7);
        }
        else if (normalizedAction.StartsWith("archive"))
        {
            command = "Archive";
            entityPart = normalizedAction.Substring(7);
        }
        else if (normalizedAction.StartsWith("activate"))
        {
            command = "Activate";
            entityPart = normalizedAction.Substring(8);
        }
        else if (normalizedAction.StartsWith("deactivate"))
        {
            command = "Deactivate";
            entityPart = normalizedAction.Substring(10);
        }

        var entityName = GetEntityNameFromSingularOrPlural(entityPart);
        return $"{command}{ToPascalCase(entityName)}{suffix}";
    }

    private string GetEntityNameFromPlural(string pluralForm)
    {
        // Use the pluralization service to convert plural to singular
        var singular = _pluralizationService.Singularize(pluralForm);
        return _entityDiscoveryService.GetProperEntityName(singular);
    }

    private string GetEntityNameFromSingularOrPlural(string entityPart)
    {
        // First try to get the proper entity name directly
        if (_entityDiscoveryService.IsValidEntity(entityPart))
        {
            return _entityDiscoveryService.GetProperEntityName(entityPart);
        }

        // Check if it might be plural
        return GetEntityNameFromPlural(entityPart);
    }

    private string ParseCamelCaseWords(string input)
    {
        // Handle compound words like "publishedblogposts"
        var result = Regex.Replace(input, @"([a-z])([A-Z])", "$1 $2");
        return ToPascalCase(result);
    }

    private record EntityMatch(string EntityName, string LowerEntityName);

    private string ToPascalCase(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        // Handle space-separated words
        if (input.Contains(' '))
        {
            return string.Concat(input.Split(' ')
                .Select(word => char.ToUpperInvariant(word[0]) + word[1..].ToLowerInvariant()));
        }

        // Handle single word
        return char.ToUpperInvariant(input[0]) + input[1..].ToLowerInvariant();
    }

    private string[] GetKnownEntities()
    {
        // Get entities from both registry operations and entity discovery service
        var registryEntities = _registry.GetAllOperations()
            .Select(op => op.EntityType)
            .Distinct()
            .ToArray();

        var discoveredEntities = _entityDiscoveryService.GetEntityNames().ToArray();

        return registryEntities.Concat(discoveredEntities).Distinct().ToArray();
    }

    private string[] GetAvailableActions()
    {
        var operations = _registry.GetAllOperations();
        return operations.Select(op => CreateActionName(op))
                        .ToArray();
    }

    private string CreateActionName(OperationMetadata operation)
    {
        // Convert operation metadata back to action name
        // Example: GetAllAuthorsQuery -> getallauthors
        var actionPart = operation.Action.ToLowerInvariant();
        var entityPart = operation.EntityType.ToLowerInvariant();
        
        if (actionPart.Contains("all"))
            return $"getall{_pluralizationService.Pluralize(entityPart)}";
        if (actionPart.Contains("byid"))
            return $"get{entityPart}byid";
        if (actionPart.Contains("byemail"))
            return $"get{entityPart}byemail";
        if (actionPart.StartsWith("create"))
            return $"create{entityPart}";
        if (actionPart.StartsWith("update"))
            return $"update{entityPart}";
        if (actionPart.StartsWith("delete"))
            return $"delete{entityPart}";
            
        return $"{actionPart}{entityPart}";
    }
}