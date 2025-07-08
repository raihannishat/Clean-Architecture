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
}