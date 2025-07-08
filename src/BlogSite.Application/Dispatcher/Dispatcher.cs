using System.Text.Json;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace BlogSite.Application.Dispatcher;

public class Dispatcher : IDispatcher
{
    private readonly IServiceProvider _provider;
    private readonly IRequestTypeRegistry _registry;

    public Dispatcher(IServiceProvider provider, IRequestTypeRegistry registry)
    {
        _provider = provider;
        _registry = registry;
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
        
        // Fallback: direct conversion
        var directConversion = ToPascalCase(normalizedAction);
        return $"{directConversion}{suffix}";
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

    private record EntityMatch(string EntityName, string LowerEntityName);

    private string ToPascalCase(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        // Split camelCase words and convert to PascalCase
        var result = string.Concat(input.Split(' ', '-', '_')
            .Select(word => char.ToUpperInvariant(word[0]) + word[1..].ToLowerInvariant()));
            
        return result;
    }

    private string[] GetKnownEntities()
    {
        // Get entities from registry operations
        var operations = _registry.GetAllOperations();
        return operations.Select(op => op.EntityType)
                        .Distinct()
                        .ToArray();
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
            return $"getall{entityPart}s";
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