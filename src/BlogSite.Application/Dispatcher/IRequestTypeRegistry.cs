namespace BlogSite.Application.Dispatcher;

/// <summary>
/// Registry for mapping operation signatures to request types
/// </summary>
public interface IRequestTypeRegistry
{
    void RegisterOperation(string operationType, string entityType, string action, Type requestType, Type? responseType = null);
    OperationMetadata? GetOperation(string operationType, string entityType, string action);
    IEnumerable<OperationMetadata> GetAllOperations();
    bool IsRegistered(string operationType, string entityType, string action);
}