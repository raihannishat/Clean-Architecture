using MediatR;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace BlogSite.Application.Dispatcher;

/// <summary>
/// Core dispatcher implementation that dynamically handles requests with convention-based routing
/// </summary>
public class Dispatcher : IDispatcher
{
    private readonly IMediator _mediator;
    private readonly IRequestTypeRegistry _registry;
    private readonly ILogger<Dispatcher> _logger;

    public Dispatcher(IMediator mediator, IRequestTypeRegistry registry, ILogger<Dispatcher> logger)
    {
        _mediator = mediator;
        _registry = registry;
        _logger = logger;
    }

    public async Task<DispatchResult> DispatchAsync(DispatchRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Dispatching {EntityType}.{Action} (auto-detected as {OperationType})", 
                request.EntityType, request.Action, request.OperationType);

            // Try to resolve the operation using the new dynamic approach
            var operation = _registry.GetOperation(request.OperationType, request.EntityType, request.Action);
            if (operation == null)
            {
                // Fallback: try to resolve by the full type name directly
                var requestType = _registry.ResolveRequestType(request.RequestTypeName);
                if (requestType == null)
                {
                    _logger.LogWarning("Operation not found: {EntityType}.{Action} (expected type: {RequestTypeName})", 
                        request.EntityType, request.Action, request.RequestTypeName);
                    
                    return new DispatchResult(
                        Success: false,
                        ErrorMessage: $"Operation '{request.EntityType}.{request.Action}' not found. Expected type: {request.RequestTypeName}",
                        ErrorCode: "OPERATION_NOT_FOUND"
                    );
                }

                // Create operation metadata from resolved type
                operation = new OperationMetadata(
                    request.OperationType, 
                    request.EntityType, 
                    request.Action, 
                    requestType, 
                    GetResponseType(requestType)
                );
            }

            // Create the request instance
            var requestInstance = CreateRequestInstance(operation.RequestType, request);
            if (requestInstance == null)
            {
                return new DispatchResult(
                    Success: false,
                    ErrorMessage: "Failed to create request instance",
                    ErrorCode: "REQUEST_CREATION_FAILED"
                );
            }

            // Send the request through MediatR
            var result = await _mediator.Send(requestInstance, cancellationToken);

            _logger.LogInformation("Successfully dispatched {EntityType}.{Action} as {OperationType}", 
                request.EntityType, request.Action, request.OperationType);

            return new DispatchResult(
                Success: true,
                Data: result,
                Metadata: new Dictionary<string, object>
                {
                    ["OperationType"] = operation.OperationType,
                    ["EntityType"] = operation.EntityType,
                    ["Action"] = operation.Action,
                    ["RequestTypeName"] = operation.RequestType.Name,
                    ["ResponseType"] = operation.ResponseType?.Name ?? "void",
                    ["Convention"] = $"Action '{request.Action}' auto-detected as {request.OperationType}"
                }
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error dispatching {EntityType}.{Action} as {OperationType}", 
                request.EntityType, request.Action, request.OperationType);

            return new DispatchResult(
                Success: false,
                ErrorMessage: ex.Message,
                ErrorCode: "DISPATCH_ERROR"
            );
        }
    }

    private object? CreateRequestInstance(Type requestType, DispatchRequest request)
    {
        try
        {
            // Handle requests with no payload (like GetAllAuthorsQuery)
            if (request.Payload == null || request.Payload.Value.ValueKind == JsonValueKind.Null)
            {
                // Try to create instance with parameterless constructor
                return TryCreateParameterlessInstance(requestType, request);
            }

            // Handle requests with payload
            var instance = JsonSerializer.Deserialize(request.Payload.Value, requestType, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            // If the instance was created but has parameters from route (like ID), we might need to reconstruct it
            return EnrichInstanceWithParameters(instance, requestType, request);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create request instance for type {RequestType}", requestType.Name);
            return null;
        }
    }

    private object? TryCreateParameterlessInstance(Type requestType, DispatchRequest request)
    {
        // Check if it's a parameterless constructor
        var parameterlessConstructor = requestType.GetConstructors()
            .FirstOrDefault(c => c.GetParameters().Length == 0);

        if (parameterlessConstructor != null)
        {
            return Activator.CreateInstance(requestType);
        }

        // Check if it's a record with parameters from the route
        var constructors = requestType.GetConstructors();
        var constructor = constructors.FirstOrDefault();

        if (constructor != null && request.Parameters != null)
        {
            var parameters = constructor.GetParameters();
            var args = new object?[parameters.Length];

            for (int i = 0; i < parameters.Length; i++)
            {
                var param = parameters[i];
                if (request.Parameters.TryGetValue(param.Name!, out var value))
                {
                    args[i] = Convert.ChangeType(value, param.ParameterType);
                }
                else if (param.HasDefaultValue)
                {
                    args[i] = param.DefaultValue;
                }
                else
                {
                    args[i] = GetDefaultValue(param.ParameterType);
                }
            }

            return Activator.CreateInstance(requestType, args);
        }

        return null;
    }

    private object? EnrichInstanceWithParameters(object? instance, Type requestType, DispatchRequest request)
    {
        if (instance == null || request.Parameters == null || request.Parameters.Count == 0)
        {
            return instance;
        }

        // For records, we might need to recreate with route parameters
        if (requestType.GetMethod("<Clone>$") != null) // This is a record type
        {
            var constructor = requestType.GetConstructors().FirstOrDefault();
            if (constructor != null)
            {
                var parameters = constructor.GetParameters();
                var args = new object?[parameters.Length];

                for (int i = 0; i < parameters.Length; i++)
                {
                    var param = parameters[i];
                    
                    // First check route parameters
                    if (request.Parameters.TryGetValue(param.Name!, out var routeValue))
                    {
                        args[i] = Convert.ChangeType(routeValue, param.ParameterType);
                    }
                    // Then check the existing instance properties
                    else
                    {
                        var property = requestType.GetProperty(param.Name!, StringComparison.OrdinalIgnoreCase);
                        if (property != null)
                        {
                            args[i] = property.GetValue(instance);
                        }
                        else if (param.HasDefaultValue)
                        {
                            args[i] = param.DefaultValue;
                        }
                        else
                        {
                            args[i] = GetDefaultValue(param.ParameterType);
                        }
                    }
                }

                return Activator.CreateInstance(requestType, args);
            }
        }

        return instance;
    }

    private static object? GetDefaultValue(Type type)
    {
        return type.IsValueType ? Activator.CreateInstance(type) : null;
    }

    private Type? GetResponseType(Type requestType)
    {
        var requestInterface = requestType.GetInterfaces()
            .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequest<>));

        return requestInterface?.GetGenericArguments().FirstOrDefault();
    }
}