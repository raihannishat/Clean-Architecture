using System.Reflection;

namespace BlogSite.Application.Core.Dynamic;

/// <summary>
/// Metadata about discovered operations
/// </summary>
public record OperationMetadata
{
    public string Name { get; init; } = string.Empty;
    public string Action { get; init; } = string.Empty;
    public Type HandlerType { get; init; } = typeof(object);
    public Type RequestType { get; init; } = typeof(object);
    public Type ResponseType { get; init; } = typeof(object);
    public MethodInfo? HandlerMethod { get; init; }
    public OperationType Type { get; init; }
    public string Description { get; init; } = string.Empty;
    public Dictionary<string, object> Tags { get; init; } = new();
    public bool RequiresAuthentication { get; init; }
    public string[] Permissions { get; init; } = Array.Empty<string>();
}

/// <summary>
/// Types of operations that can be discovered
/// </summary>
public enum OperationType
{
    Command,
    Query,
    Event,
    Handler,
    Service,
    Unknown
}