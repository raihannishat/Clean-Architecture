using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MediatR;

namespace BlogSite.Application.Core.Dynamic;

/// <summary>
/// Implementation of dynamic operation discovery service
/// </summary>
public class OperationDiscoveryService : IOperationDiscoveryService
{
    private readonly ILogger<OperationDiscoveryService> _logger;
    private static readonly Dictionary<string, OperationMetadata> _discoveredOperations = new();

    public OperationDiscoveryService(ILogger<OperationDiscoveryService> logger)
    {
        _logger = logger;
    }

    public async Task<IEnumerable<OperationMetadata>> DiscoverOperationsAsync()
    {
        var operations = new List<OperationMetadata>();
        
        // Get all loaded assemblies
        var assemblies = AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => !a.IsDynamic && a.FullName?.Contains("BlogSite") == true);

        foreach (var assembly in assemblies)
        {
            var assemblyOperations = await DiscoverOperationsAsync(assembly);
            operations.AddRange(assemblyOperations);
        }

        _logger.LogInformation("Discovered {Count} operations", operations.Count);
        return operations;
    }

    public async Task<IEnumerable<OperationMetadata>> DiscoverOperationsAsync(Assembly assembly)
    {
        var operations = new List<OperationMetadata>();

        try
        {
            // Discover MediatR requests (Commands and Queries)
            var requestTypes = assembly.GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && IsMediatrRequest(t));

            foreach (var requestType in requestTypes)
            {
                var operation = await CreateOperationMetadataAsync(requestType);
                if (operation != null)
                {
                    operations.Add(operation);
                    _discoveredOperations[operation.Action] = operation;
                }
            }

            // Discover service methods
            var serviceTypes = assembly.GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && t.Name.EndsWith("Service"));

            foreach (var serviceType in serviceTypes)
            {
                var serviceMethods = serviceType.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                    .Where(m => !m.IsSpecialName && m.DeclaringType == serviceType);

                foreach (var method in serviceMethods)
                {
                    var operation = await CreateServiceOperationMetadataAsync(serviceType, method);
                    if (operation != null)
                    {
                        operations.Add(operation);
                        _discoveredOperations[operation.Action] = operation;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error discovering operations from assembly {AssemblyName}", assembly.FullName);
        }

        return operations;
    }

    public async Task RegisterDiscoveredOperationsAsync(IServiceCollection services)
    {
        var operations = await DiscoverOperationsAsync();
        
        foreach (var operation in operations)
        {
            // Register the operation handler if it's not already registered
            if (operation.HandlerType != typeof(object))
            {
                if (!services.Any(x => x.ServiceType == operation.HandlerType))
                {
                    services.AddScoped(operation.HandlerType);
                }
            }
        }

        _logger.LogInformation("Registered {Count} operations", operations.Count());
    }

    private static bool IsMediatrRequest(Type type)
    {
        return type.GetInterfaces().Any(i => 
            i.IsGenericType && 
            (i.GetGenericTypeDefinition() == typeof(IRequest<>) || 
             i.GetGenericTypeDefinition() == typeof(IRequest)));
    }

    private async Task<OperationMetadata?> CreateOperationMetadataAsync(Type requestType)
    {
        try
        {
            var action = GenerateActionName(requestType);
            var operationType = DetermineOperationType(requestType);
            var responseType = GetResponseType(requestType);
            var handlerType = GetHandlerType(requestType, responseType);

            return new OperationMetadata
            {
                Name = requestType.Name,
                Action = action,
                HandlerType = handlerType,
                RequestType = requestType,
                ResponseType = responseType,
                Type = operationType,
                Description = GenerateDescription(requestType, action),
                Tags = GenerateTags(requestType),
                RequiresAuthentication = HasAuthenticationAttribute(requestType)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating operation metadata for {TypeName}", requestType.Name);
            return null;
        }
    }

    private async Task<OperationMetadata?> CreateServiceOperationMetadataAsync(Type serviceType, MethodInfo method)
    {
        try
        {
            var action = GenerateActionNameFromMethod(serviceType, method);
            var operationType = method.Name.StartsWith("Get") ? OperationType.Query : OperationType.Command;

            return new OperationMetadata
            {
                Name = $"{serviceType.Name}.{method.Name}",
                Action = action,
                HandlerType = serviceType,
                RequestType = method.GetParameters().FirstOrDefault()?.ParameterType ?? typeof(object),
                ResponseType = method.ReturnType,
                HandlerMethod = method,
                Type = operationType,
                Description = GenerateMethodDescription(serviceType, method),
                Tags = GenerateMethodTags(serviceType, method)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating service operation metadata for {ServiceType}.{MethodName}", 
                serviceType.Name, method.Name);
            return null;
        }
    }

    private string GenerateActionName(Type requestType)
    {
        var name = requestType.Name;
        
        // Remove common suffixes
        var suffixes = new[] { "Command", "Query", "Request" };
        foreach (var suffix in suffixes)
        {
            if (name.EndsWith(suffix))
            {
                name = name.Substring(0, name.Length - suffix.Length);
                break;
            }
        }

        // Convert to lowercase action
        return ConvertToActionFormat(name);
    }

    private string GenerateActionNameFromMethod(Type serviceType, MethodInfo method)
    {
        var serviceName = serviceType.Name.Replace("Service", "");
        var methodName = method.Name;
        
        return ConvertToActionFormat($"{methodName}{serviceName}");
    }

    private string ConvertToActionFormat(string input)
    {
        // Convert PascalCase to lowercase with dots
        var result = string.Empty;
        for (int i = 0; i < input.Length; i++)
        {
            if (i > 0 && char.IsUpper(input[i]))
            {
                result += ".";
            }
            result += char.ToLower(input[i]);
        }
        return result;
    }

    private OperationType DetermineOperationType(Type requestType)
    {
        var name = requestType.Name.ToLower();
        
        if (name.Contains("command")) return OperationType.Command;
        if (name.Contains("query")) return OperationType.Query;
        if (name.Contains("event")) return OperationType.Event;
        
        return OperationType.Unknown;
    }

    private Type GetResponseType(Type requestType)
    {
        var requestInterface = requestType.GetInterfaces()
            .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequest<>));
        
        return requestInterface?.GetGenericArguments()[0] ?? typeof(object);
    }

    private Type GetHandlerType(Type requestType, Type responseType)
    {
        var handlerInterfaceType = typeof(IRequestHandler<,>).MakeGenericType(requestType, responseType);
        
        var assemblies = AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => !a.IsDynamic && a.FullName?.Contains("BlogSite") == true);

        foreach (var assembly in assemblies)
        {
            var handlerType = assembly.GetTypes()
                .FirstOrDefault(t => t.IsClass && !t.IsAbstract && 
                               handlerInterfaceType.IsAssignableFrom(t));
            
            if (handlerType != null)
                return handlerType;
        }

        return typeof(object);
    }

    private string GenerateDescription(Type requestType, string action)
    {
        var operationType = DetermineOperationType(requestType);
        var entityName = ExtractEntityName(requestType.Name);
        
        return operationType switch
        {
            OperationType.Command => $"Executes {action} command on {entityName}",
            OperationType.Query => $"Executes {action} query for {entityName}",
            _ => $"Executes {action} operation"
        };
    }

    private string GenerateMethodDescription(Type serviceType, MethodInfo method)
    {
        var serviceName = serviceType.Name.Replace("Service", "");
        return $"Executes {method.Name} operation on {serviceName} service";
    }

    private string ExtractEntityName(string requestName)
    {
        var suffixes = new[] { "Command", "Query", "Request" };
        var prefixes = new[] { "Create", "Update", "Delete", "Get", "Remove" };
        
        var name = requestName;
        
        // Remove suffixes
        foreach (var suffix in suffixes)
        {
            if (name.EndsWith(suffix))
            {
                name = name.Substring(0, name.Length - suffix.Length);
                break;
            }
        }
        
        // Remove prefixes
        foreach (var prefix in prefixes)
        {
            if (name.StartsWith(prefix))
            {
                name = name.Substring(prefix.Length);
                break;
            }
        }
        
        return name;
    }

    private Dictionary<string, object> GenerateTags(Type requestType)
    {
        var tags = new Dictionary<string, object>
        {
            ["assembly"] = requestType.Assembly.GetName().Name ?? "Unknown",
            ["namespace"] = requestType.Namespace ?? "Unknown",
            ["type"] = DetermineOperationType(requestType).ToString()
        };

        return tags;
    }

    private Dictionary<string, object> GenerateMethodTags(Type serviceType, MethodInfo method)
    {
        var tags = new Dictionary<string, object>
        {
            ["assembly"] = serviceType.Assembly.GetName().Name ?? "Unknown",
            ["namespace"] = serviceType.Namespace ?? "Unknown",
            ["service"] = serviceType.Name,
            ["method"] = method.Name,
            ["type"] = method.Name.StartsWith("Get") ? "Query" : "Command"
        };

        return tags;
    }

    private bool HasAuthenticationAttribute(Type requestType)
    {
        return requestType.GetCustomAttributes().Any(attr => 
            attr.GetType().Name.Contains("Authorize") || 
            attr.GetType().Name.Contains("Auth"));
    }

    public static OperationMetadata? GetOperation(string action)
    {
        _discoveredOperations.TryGetValue(action, out var operation);
        return operation;
    }

    public static IEnumerable<OperationMetadata> GetAllOperations()
    {
        return _discoveredOperations.Values;
    }
}