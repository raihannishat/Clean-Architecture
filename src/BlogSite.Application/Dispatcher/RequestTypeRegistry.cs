using System.Collections.Concurrent;
using System.Reflection;
using MediatR;
using Microsoft.Extensions.Options;
using BlogSite.Application.Services;
using BlogSite.Application.Configuration;

namespace BlogSite.Application.Dispatcher;

/// <summary>
/// Dynamic registry that resolves request types by convention
/// </summary>
public class RequestTypeRegistry : IRequestTypeRegistry
{
    private readonly ConcurrentDictionary<string, Type> _typeCache = new();
    private readonly ConcurrentDictionary<string, OperationMetadata> _operationCache = new();
    private readonly Assembly[] _assemblies;
    private readonly IEntityDiscoveryService? _entityDiscoveryService;
    private readonly EntityDiscoveryOptions _entityDiscoveryOptions;

    public RequestTypeRegistry(IEntityDiscoveryService? entityDiscoveryService = null, IOptions<EntityDiscoveryOptions>? entityDiscoveryOptions = null)
    {
        _entityDiscoveryService = entityDiscoveryService;
        _entityDiscoveryOptions = entityDiscoveryOptions?.Value ?? new EntityDiscoveryOptions();
        
        // Load assemblies that might contain commands and queries
        _assemblies = new[]
        {
            Assembly.GetExecutingAssembly(), // Current assembly (Application)
            Assembly.GetCallingAssembly(),   // Assembly that called this
        };
    }

    public Type? ResolveRequestType(string requestTypeName)
    {
        if (string.IsNullOrEmpty(requestTypeName))
            return null;

        // Check cache first
        if (_typeCache.TryGetValue(requestTypeName, out var cachedType))
            return cachedType;

        // Search for the type in loaded assemblies
        foreach (var assembly in _assemblies)
        {
            var type = assembly.GetTypes()
                .FirstOrDefault(t => 
                    t.Name.Equals(requestTypeName, StringComparison.OrdinalIgnoreCase) ||
                    t.FullName?.EndsWith($".{requestTypeName}", StringComparison.OrdinalIgnoreCase) == true);

            if (type != null && IsValidRequestType(type))
            {
                _typeCache.TryAdd(requestTypeName, type);
                return type;
            }
        }

        return null;
    }

    public OperationMetadata? GetOperation(string operationType, string entityType, string action)
    {
        var key = CreateKey(operationType, entityType, action);
        
        if (_operationCache.TryGetValue(key, out var cached))
            return cached;

        // Create the expected type name based on convention
        var requestTypeName = $"{action}{entityType}{operationType}";
        var requestType = ResolveRequestType(requestTypeName);
        
        if (requestType == null)
            return null;

        // Try to find response type
        var responseType = GetResponseType(requestType);
        
        var metadata = new OperationMetadata(operationType, entityType, action, requestType, responseType);
        _operationCache.TryAdd(key, metadata);
        
        return metadata;
    }

    public IEnumerable<OperationMetadata> GetAllOperations()
    {
        var operations = new List<OperationMetadata>();

        foreach (var assembly in _assemblies)
        {
            var requestTypes = assembly.GetTypes()
                .Where(IsValidRequestType)
                .ToList();

            foreach (var type in requestTypes)
            {
                var metadata = ParseTypeNameToMetadata(type);
                if (metadata != null)
                {
                    operations.Add(metadata);
                }
            }
        }

        return operations.DistinctBy(op => $"{op.OperationType}:{op.EntityType}:{op.Action}");
    }

    private bool IsValidRequestType(Type type)
    {
        if (type.IsAbstract || type.IsInterface || type.IsGenericTypeDefinition)
            return false;

        // Check if it implements IRequest or IRequest<T>
        return typeof(IRequest).IsAssignableFrom(type) || 
               type.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequest<>));
    }

    private Type? GetResponseType(Type requestType)
    {
        // Check if it implements IRequest<T>
        var requestInterface = requestType.GetInterfaces()
            .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequest<>));

        return requestInterface?.GetGenericArguments().FirstOrDefault();
    }

    private OperationMetadata? ParseTypeNameToMetadata(Type type)
    {
        var typeName = type.Name;
        
        // Try to parse the type name: {Action}{Entity}{OperationType}
        // Examples: GetAllAuthorsQuery, CreateAuthorCommand
        
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
            return null;
        }

        // Extract entity and action from remainingName
        var result = ExtractEntityAndAction(remainingName);
        if (result == null)
            return null;

        var (entity, action) = result.Value;
        var responseType = GetResponseType(type);
        return new OperationMetadata(operationType, entity, action, type, responseType);
    }

    /// <summary>
    /// Dynamically extracts entity and action from the remaining type name
    /// </summary>
    private (string Entity, string Action)? ExtractEntityAndAction(string remainingName)
    {
        if (string.IsNullOrEmpty(remainingName))
            return null;

        // Get known entities from EntityDiscoveryService if available, 
        // otherwise discover from existing types
        var knownEntities = GetKnownEntities();
        
        // Try to match known entities (longest first to handle composite entities like "BlogPost")
        foreach (var entity in knownEntities.OrderByDescending(e => e.Length))
        {
            if (remainingName.EndsWith(entity, StringComparison.OrdinalIgnoreCase))
            {
                var action = remainingName[..^entity.Length];
                if (!string.IsNullOrEmpty(action))
                {
                    return (entity, action);
                }
            }
        }

        // If no known entity matches, try to infer the entity dynamically
        // Look for common patterns and plurals
        return InferEntityFromPattern(remainingName);
    }

    /// <summary>
    /// Gets known entities from EntityDiscoveryService using multiple discovery strategies
    /// </summary>
    private HashSet<string> GetKnownEntities()
    {
        var entities = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        // Primary: Use EntityDiscoveryService if available
        if (_entityDiscoveryService != null)
        {
            try
            {
                var discoveredEntities = _entityDiscoveryService.GetEntityNames();
                entities.UnionWith(discoveredEntities);
                
                // Also trigger async discovery for future requests
                _ = Task.Run(async () => await _entityDiscoveryService.DiscoverEntitiesFromRequestTypesAsync());
            }
            catch
            {
                // Continue with fallback strategies
            }
        }

        // Secondary: Discover from existing command/query types in current assemblies
        DiscoverEntitiesFromAssemblies(entities);
        
        // Tertiary: Use configured fallback entities only if absolutely nothing found
        if (!entities.Any() && _entityDiscoveryOptions.FallbackEntities.Any())
        {
            entities.UnionWith(_entityDiscoveryOptions.FallbackEntities);
            
            // Register discovered fallback entities for future use
            if (_entityDiscoveryService != null)
            {
                foreach (var fallbackEntity in _entityDiscoveryOptions.FallbackEntities)
                {
                    _entityDiscoveryService.RegisterEntity(fallbackEntity);
                }
            }
        }

        return entities;
    }

    /// <summary>
    /// Discovers entities from assemblies by analyzing request type patterns
    /// </summary>
    private void DiscoverEntitiesFromAssemblies(HashSet<string> entities)
    {
        foreach (var assembly in _assemblies)
        {
            try
            {
                var commandQueryTypes = assembly.GetTypes()
                    .Where(t => IsValidRequestType(t) && 
                              (t.Name.EndsWith("Command") || t.Name.EndsWith("Query")))
                    .ToList();

                foreach (var type in commandQueryTypes)
                {
                    var entity = ExtractEntityFromTypeName(type.Name);
                    if (!string.IsNullOrEmpty(entity))
                    {
                        entities.Add(entity);
                        
                        // Auto-register discovered entities
                        _entityDiscoveryService?.RegisterEntity(entity);
                    }
                }
            }
            catch
            {
                // Continue with other assemblies
            }
        }
    }

    /// <summary>
    /// Extracts entity name from a command/query type name
    /// </summary>
    private string? ExtractEntityFromTypeName(string typeName)
    {
        // Remove Command/Query suffix
        if (typeName.EndsWith("Command"))
            typeName = typeName[..^7];
        else if (typeName.EndsWith("Query"))
            typeName = typeName[..^5];

        // Try to find entity by looking for capitalized words at the end
        var words = SplitCamelCase(typeName);
        
        // The entity is typically one of the last words
        for (int i = words.Count - 1; i >= 0; i--)
        {
            var potentialEntity = string.Join("", words.Skip(i));
            
            // Check if this looks like an entity (starts with capital, reasonable length)
            if (potentialEntity.Length > 2 && 
                char.IsUpper(potentialEntity[0]) &&
                potentialEntity.Length <= 20)
            {
                return potentialEntity;
            }
        }

        return null;
    }

    /// <summary>
    /// Infers entity from pattern when no known entity matches
    /// </summary>
    private (string Entity, string Action)? InferEntityFromPattern(string remainingName)
    {
        var words = SplitCamelCase(remainingName);
        
        if (words.Count < 2)
            return null;

        // Assume the last word (or last few words) is the entity
        // and everything before is the action
        for (int entityWordCount = 1; entityWordCount <= Math.Min(3, words.Count - 1); entityWordCount++)
        {
            var entity = string.Join("", words.TakeLast(entityWordCount));
            var action = string.Join("", words.Take(words.Count - entityWordCount));
            
            if (!string.IsNullOrEmpty(action) && !string.IsNullOrEmpty(entity))
            {
                return (entity, action);
            }
        }

        return null;
    }

    /// <summary>
    /// Splits a camelCase/PascalCase string into individual words
    /// </summary>
    private List<string> SplitCamelCase(string input)
    {
        var words = new List<string>();
        var currentWord = new System.Text.StringBuilder();

        for (int i = 0; i < input.Length; i++)
        {
            char c = input[i];
            
            if (char.IsUpper(c) && currentWord.Length > 0)
            {
                words.Add(currentWord.ToString());
                currentWord.Clear();
            }
            
            currentWord.Append(c);
        }

        if (currentWord.Length > 0)
        {
            words.Add(currentWord.ToString());
        }

        return words;
    }

    private static string CreateKey(string operationType, string entityType, string action)
    {
        return $"{operationType.ToLowerInvariant()}:{entityType.ToLowerInvariant()}:{action.ToLowerInvariant()}";
    }
}