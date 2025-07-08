using System.Collections.Concurrent;

namespace BlogSite.Application.Dispatcher;

/// <summary>
/// Thread-safe registry for request type mappings
/// </summary>
public class RequestTypeRegistry : IRequestTypeRegistry
{
    private readonly ConcurrentDictionary<string, OperationMetadata> _operations = new();

    public void RegisterOperation(string operationType, string entityType, string action, Type requestType, Type? responseType = null)
    {
        var key = CreateKey(operationType, entityType, action);
        var metadata = new OperationMetadata(operationType, entityType, action, requestType, responseType);
        _operations.TryAdd(key, metadata);
    }

    public OperationMetadata? GetOperation(string operationType, string entityType, string action)
    {
        var key = CreateKey(operationType, entityType, action);
        return _operations.TryGetValue(key, out var metadata) ? metadata : null;
    }

    public IEnumerable<OperationMetadata> GetAllOperations()
    {
        return _operations.Values;
    }

    public bool IsRegistered(string operationType, string entityType, string action)
    {
        var key = CreateKey(operationType, entityType, action);
        return _operations.ContainsKey(key);
    }

    private static string CreateKey(string operationType, string entityType, string action)
    {
        return $"{operationType.ToLowerInvariant()}:{entityType.ToLowerInvariant()}:{action.ToLowerInvariant()}";
    }
}