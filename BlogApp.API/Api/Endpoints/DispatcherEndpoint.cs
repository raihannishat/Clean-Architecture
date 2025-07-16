namespace BlogApp.API.Api.Endpoints;

public class DispatcherEndpoint : Endpoint<DispatcherRequest, DispatcherResponse>
{
    private readonly IMediator _mediator;
    private readonly IServiceProvider _serviceProvider;

    public DispatcherEndpoint(IMediator mediator, IServiceProvider serviceProvider)
    {
        _mediator = mediator;
        _serviceProvider = serviceProvider;
    }

    public override void Configure()
    {
        Post("/api/dispatcher");
        AllowAnonymous();
    }

    public override async Task HandleAsync(DispatcherRequest req, CancellationToken ct)
    {
        try
        {
            // Try to resolve the type by appending 'Command' or 'Query' to the operation name
            var operationType = FindOperationType(req.OperationName);
            if (operationType == null)
            {
                await SendAsync(new DispatcherResponse
                {
                    Success = false,
                    Message = $"Operation '{req.OperationName}' not found",
                    Data = null
                }, 404, ct);
                return;
            }

            var request = System.Text.Json.JsonSerializer.Deserialize(req.Data, operationType);
            if (request == null)
            {
                await SendAsync(new DispatcherResponse
                {
                    Success = false,
                    Message = "Failed to deserialize request data",
                    Data = null
                }, 400, ct);
                return;
            }

            var validatorType = typeof(IValidator<>).MakeGenericType(operationType);
            var validator = _serviceProvider.GetService(validatorType) as IValidator;

            if (validator != null)
            {
                var validationContext = new FluentValidation.ValidationContext<object>(request);
                var validationResult = await validator.ValidateAsync(validationContext, ct);

                if (!validationResult.IsValid)
                {
                    var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                    await SendAsync(new DispatcherResponse
                    {
                        Success = false,
                        Message = "Validation failed",
                        Data = errors
                    }, 400, ct);
                    return;
                }
            }

            var responseType = GetResponseType(operationType);
            if (responseType == null)
            {
                await SendAsync(new DispatcherResponse
                {
                    Success = false,
                    Message = "Could not determine response type",
                    Data = null
                }, 400, ct);
                return;
            }

            var method = typeof(IMediator).GetMethod("SendAsync");
            var genericMethod = method!.MakeGenericMethod(operationType, responseType);
            var task = (Task)genericMethod.Invoke(_mediator, new object[] { request, ct })!;
            await task.ConfigureAwait(false);

            var resultProperty = task.GetType().GetProperty("Result");
            var result = resultProperty!.GetValue(task);

            if (result is BaseResponse baseResponse)
            {
                await SendAsync(new DispatcherResponse
                {
                    Success = baseResponse.IsSuccess,
                    Message = baseResponse.Message,
                    Data = baseResponse.Data,
                    Errors = baseResponse.Errors,
                    StatusCode = baseResponse.StatusCode
                }, baseResponse.StatusCode, ct);
            }
            else
            {
                await SendAsync(new DispatcherResponse
                {
                    Success = true,
                    Message = "Operation completed successfully",
                    Data = result
                }, 200, ct);
            }
        }
        catch (Exception ex)
        {
            await SendAsync(new DispatcherResponse
            {
                Success = false,
                Message = $"Internal server error: {ex.Message}",
                Data = null
            }, 500, ct);
        }
    }

    private static readonly Dictionary<string, Type> OperationTypeCache = BuildOperationTypeCache();

    private static Dictionary<string, Type> BuildOperationTypeCache()
    {
        var dict = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        foreach (var assembly in assemblies)
        {
            foreach (var type in assembly.GetTypes())
            {
                if (!type.IsClass && !type.IsValueType) continue;
                if (type.Name.EndsWith("Command") || type.Name.EndsWith("Query"))
                {
                    var opName = type.Name;
                    if (opName.EndsWith("Command"))
                        opName = opName.Substring(0, opName.Length - "Command".Length);
                    else if (opName.EndsWith("Query"))
                        opName = opName.Substring(0, opName.Length - "Query".Length);
                    if (!dict.ContainsKey(opName))
                        dict[opName] = type;
                }
            }
        }
        return dict;
    }

    private Type? FindOperationType(string operationName)
    {
        return OperationTypeCache.TryGetValue(operationName, out var type) ? type : null;
    }

    private Type? GetResponseType(Type operationType)
    {
        var commandInterface = operationType.GetInterfaces()
            .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(BlogApp.API.Application.CQRS.ICommand<>));
        
        if (commandInterface != null)
        {
            return commandInterface.GetGenericArguments()[0];
        }

        var queryInterface = operationType.GetInterfaces()
            .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(BlogApp.API.Application.CQRS.IQuery<>));
        
        return queryInterface?.GetGenericArguments()[0];
    }
}

public class DispatcherRequest
{
    public string OperationName { get; set; } = string.Empty;
    public string Data { get; set; } = string.Empty;
}

public class DispatcherResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public object? Data { get; set; }
    public List<string>? Errors { get; set; }
    public int StatusCode { get; set; } = 200;
} 