namespace BlogSite.Application.Dispatcher;

/// <summary>
/// Enhanced registry that supports dynamic type resolution
/// </summary>
public interface IRequestTypeRegistry
{
    /// <summary>
    /// Dynamically resolves a request type by convention
    /// </summary>
    Type? ResolveRequestType(string requestTypeName);
    
    /// <summary>
    /// Gets operation metadata by convention
    /// </summary>
    OperationMetadata? GetOperation(string operationType, string entityType, string action);
    
    /// <summary>
    /// Gets all available operations from loaded assemblies
    /// </summary>
    IEnumerable<OperationMetadata> GetAllOperations();
    
    /// <summary>
    /// Registers an operation manually (optional - auto-discovery is preferred)
    /// </summary>
    void RegisterOperation(string operationType, string entityType, string action, Type requestType, Type? responseType);
}