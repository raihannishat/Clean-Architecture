using System.Reflection;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MediatR;

namespace BlogSite.Application.Core.Dynamic;

/// <summary>
/// Dynamic dispatcher implementation that handles operations without hardcoded configuration
/// </summary>
public class DynamicDispatcher : IDynamicDispatcher
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<DynamicDispatcher> _logger;
    private readonly IOperationDiscoveryService _discoveryService;

    public DynamicDispatcher(
        IServiceProvider serviceProvider,
        ILogger<DynamicDispatcher> logger,
        IOperationDiscoveryService discoveryService)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _discoveryService = discoveryService;
    }

    public async Task<object?> DispatchAsync(string action, JsonElement payload, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Dispatching action: {Action}", action);

            var operation = await GetOperationMetadataAsync(action);
            if (operation == null)
            {
                _logger.LogWarning("Operation not found: {Action}", action);
                throw new InvalidOperationException($"Operation '{action}' not found");
            }

            return await ExecuteOperationAsync(operation, payload, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error dispatching action: {Action}", action);
            throw;
        }
    }

    public async Task<TResult> DispatchAsync<TResult>(string action, object payload, CancellationToken cancellationToken = default)
    {
        var jsonPayload = JsonSerializer.SerializeToElement(payload);
        var result = await DispatchAsync(action, jsonPayload, cancellationToken);
        
        if (result is TResult typedResult)
        {
            return typedResult;
        }

        if (result != null)
        {
            var serialized = JsonSerializer.Serialize(result);
            return JsonSerializer.Deserialize<TResult>(serialized) ?? default(TResult)!;
        }

        return default(TResult)!;
    }

    public async Task<IEnumerable<OperationMetadata>> GetAvailableOperationsAsync()
    {
        return await _discoveryService.DiscoverOperationsAsync();
    }

    public async Task<OperationMetadata?> GetOperationMetadataAsync(string action)
    {
        var operations = await _discoveryService.DiscoverOperationsAsync();
        return operations.FirstOrDefault(op => op.Action.Equals(action, StringComparison.OrdinalIgnoreCase));
    }

    public async Task<bool> OperationExistsAsync(string action)
    {
        var operation = await GetOperationMetadataAsync(action);
        return operation != null;
    }

    private async Task<object?> ExecuteOperationAsync(OperationMetadata operation, JsonElement payload, CancellationToken cancellationToken)
    {
        try
        {
            // Handle MediatR operations
            if (IsMediatrOperation(operation))
            {
                return await ExecuteMediatrOperationAsync(operation, payload, cancellationToken);
            }

            // Handle service method operations
            if (operation.HandlerMethod != null)
            {
                return await ExecuteServiceMethodAsync(operation, payload, cancellationToken);
            }

            throw new NotSupportedException($"Operation type not supported: {operation.Type}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing operation: {OperationName}", operation.Name);
            throw;
        }
    }

    private bool IsMediatrOperation(OperationMetadata operation)
    {
        return operation.RequestType.GetInterfaces().Any(i =>
            i.IsGenericType &&
            (i.GetGenericTypeDefinition() == typeof(IRequest<>) ||
             i.GetGenericTypeDefinition() == typeof(IRequest)));
    }

    private async Task<object?> ExecuteMediatrOperationAsync(OperationMetadata operation, JsonElement payload, CancellationToken cancellationToken)
    {
        var mediator = _serviceProvider.GetRequiredService<IMediator>();
        
        // Deserialize payload to request type
        var request = JsonSerializer.Deserialize(payload, operation.RequestType);
        if (request == null)
        {
            throw new InvalidOperationException($"Failed to deserialize payload to {operation.RequestType.Name}");
        }

        // Use dynamic to call Send method - simpler than reflection
        dynamic dynamicRequest = request;
        var result = await mediator.Send(dynamicRequest, cancellationToken);
        
        return result;
    }

    private async Task<object?> ExecuteServiceMethodAsync(OperationMetadata operation, JsonElement payload, CancellationToken cancellationToken)
    {
        var serviceInstance = _serviceProvider.GetRequiredService(operation.HandlerType);
        var method = operation.HandlerMethod!;

        // Prepare method parameters
        var parameters = new List<object?>();
        var methodParams = method.GetParameters();

        if (methodParams.Length > 0)
        {
            foreach (var param in methodParams)
            {
                if (param.ParameterType == typeof(CancellationToken))
                {
                    parameters.Add(cancellationToken);
                }
                else if (param.ParameterType.IsValueType || param.ParameterType == typeof(string))
                {
                    // Handle primitive types
                    var value = JsonSerializer.Deserialize(payload, param.ParameterType);
                    parameters.Add(value);
                }
                else
                {
                    // Handle complex types
                    var value = JsonSerializer.Deserialize(payload, param.ParameterType);
                    parameters.Add(value);
                }
            }
        }

        // Invoke the method
        var result = method.Invoke(serviceInstance, parameters.ToArray());

        // Handle async methods
        if (result is Task task)
        {
            await task;
            
            if (task.GetType().IsGenericType)
            {
                var resultProperty = task.GetType().GetProperty("Result");
                return resultProperty?.GetValue(task);
            }
            
            return null;
        }

        return result;
    }
}

/// <summary>
/// Extension methods for registering the dynamic dispatcher
/// </summary>
public static class DynamicDispatcherExtensions
{
    public static IServiceCollection AddDynamicDispatcher(this IServiceCollection services)
    {
        services.AddScoped<IOperationDiscoveryService, OperationDiscoveryService>();
        services.AddScoped<IDynamicDispatcher, DynamicDispatcher>();
        
        return services;
    }

    public static async Task<IServiceCollection> RegisterDynamicOperationsAsync(this IServiceCollection services)
    {
        var serviceProvider = services.BuildServiceProvider();
        var discoveryService = serviceProvider.GetRequiredService<IOperationDiscoveryService>();
        
        await discoveryService.RegisterDiscoveredOperationsAsync(services);
        
        return services;
    }
}