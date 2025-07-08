using System.Collections.Concurrent;
using System.Reflection;
using MediatR;

namespace BlogSite.Application.Dispatcher;

/// <summary>
/// Dynamic registry that resolves request types by convention
/// </summary>
public class RequestTypeRegistry : IRequestTypeRegistry
{
    private readonly ConcurrentDictionary<string, Type> _typeCache = new();
    private readonly ConcurrentDictionary<string, OperationMetadata> _operationCache = new();
    private readonly ConcurrentDictionary<string, OperationMetadata> _manualRegistrations = new();
    private readonly Assembly[] _assemblies;

    public RequestTypeRegistry()
    {
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

        // Add manual registrations first
        operations.AddRange(_manualRegistrations.Values);

        // Auto-discover from assemblies
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

    public void RegisterOperation(string operationType, string entityType, string action, Type requestType, Type? responseType)
    {
        var key = CreateKey(operationType, entityType, action);
        var metadata = new OperationMetadata(operationType, entityType, action, requestType, responseType);
        _manualRegistrations.TryAdd(key, metadata);
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

        // Now parse {Action}{Entity} from remainingName
        // This is tricky because we need to know where Action ends and Entity begins
        // We'll use some heuristics and known entity types
        var knownEntities = new[] { "Author", "BlogPost", "Category", "Comment" };
        
        foreach (var entity in knownEntities)
        {
            if (remainingName.EndsWith(entity, StringComparison.OrdinalIgnoreCase))
            {
                var action = remainingName[..^entity.Length];
                if (!string.IsNullOrEmpty(action))
                {
                    var responseType = GetResponseType(type);
                    return new OperationMetadata(operationType, entity, action, type, responseType);
                }
            }
        }

        return null;
    }

    private static string CreateKey(string operationType, string entityType, string action)
    {
        return $"{operationType.ToLowerInvariant()}:{entityType.ToLowerInvariant()}:{action.ToLowerInvariant()}";
    }
}