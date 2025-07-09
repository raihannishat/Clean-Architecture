using System.Text.Json;

namespace BlogSite.Application.Core.Dynamic;

/// <summary>
/// Dynamic dispatcher interface for handling operations without hardcoded configuration
/// </summary>
public interface IDynamicDispatcher
{
    /// <summary>
    /// Dynamically dispatches an operation based on action name
    /// </summary>
    Task<object?> DispatchAsync(string action, JsonElement payload, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Dispatches an operation with typed payload
    /// </summary>
    Task<TResult> DispatchAsync<TResult>(string action, object payload, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets all available operations
    /// </summary>
    Task<IEnumerable<OperationMetadata>> GetAvailableOperationsAsync();
    
    /// <summary>
    /// Gets operation metadata by action name
    /// </summary>
    Task<OperationMetadata?> GetOperationMetadataAsync(string action);
    
    /// <summary>
    /// Checks if an operation exists
    /// </summary>
    Task<bool> OperationExistsAsync(string action);
}