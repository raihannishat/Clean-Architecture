using System.Text.Json;

namespace BlogSite.Application.Dispatcher;

/// <summary>
/// Core dispatcher interface for dynamic request handling
/// </summary>
public interface IDispatcher
{
    Task<DispatchResult> DispatchAsync(DispatchRequest request, CancellationToken cancellationToken = default);
}

/// <summary>
/// Generic request wrapper for dynamic dispatching with automatic operation type detection
/// </summary>
public record DispatchRequest(
    string EntityType,       // "Author", "BlogPost", "Category", "Comment"
    string Action,           // "Create", "Update", "Delete", "GetAll", "GetById", etc.
    JsonElement? Payload,    // JSON payload containing the actual request data
    Dictionary<string, object>? Parameters = null  // Additional parameters (like IDs from route)
)
{
    /// <summary>
    /// Automatically determines operation type based on action name
    /// If action starts with "Get", returns "Query", otherwise returns "Command"
    /// </summary>
    public string OperationType => Action.StartsWith("Get", StringComparison.OrdinalIgnoreCase) ? "Query" : "Command";
    
    /// <summary>
    /// Gets the full request type name with the appropriate suffix
    /// </summary>
    public string RequestTypeName => $"{Action}{EntityType}{OperationType}";
};

/// <summary>
/// Generic response wrapper for dispatched requests
/// </summary>
public record DispatchResult(
    bool Success,
    object? Data = null,
    string? ErrorMessage = null,
    string? ErrorCode = null,
    Dictionary<string, object>? Metadata = null
);

/// <summary>
/// Request metadata for operation discovery
/// </summary>
public record OperationMetadata(
    string OperationType,
    string EntityType,
    string Action,
    Type RequestType,
    Type? ResponseType = null
);