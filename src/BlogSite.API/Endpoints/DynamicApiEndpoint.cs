using System.Text.Json;
using BlogSite.Application.Core.Dynamic;
using Microsoft.AspNetCore.Mvc;

namespace BlogSite.API.Endpoints;

/// <summary>
/// Completely dynamic API endpoint using dynamic dispatcher
/// </summary>
public static class DynamicApiEndpoint
{
    public static void MapDynamicApiEndpoint(this IEndpointRouteBuilder endpoints)
    {
        // Single dynamic endpoint that handles all operations
        endpoints.MapPost("/api", HandleRequest)
            .WithName("DynamicAPI")
            .WithSummary("Universal Dynamic API Endpoint")
            .WithDescription("Single endpoint that can execute any operation dynamically discovered from the application assemblies")
            .WithTags("Dynamic API")
            .Produces<object>()
            .Produces<string>(400)
            .Produces<string>(404)
            .Produces<string>(500);

        // Operations discovery endpoint
        endpoints.MapGet("/api/operations", GetAvailableOperations)
            .WithName("GetOperations")
            .WithSummary("Get all available operations")
            .WithDescription("Returns metadata for all dynamically discovered operations")
            .WithTags("Dynamic API")
            .Produces<IEnumerable<object>>();

        // Operation metadata endpoint
        endpoints.MapGet("/api/operations/{action}", GetOperationMetadata)
            .WithName("GetOperationMetadata")
            .WithSummary("Get operation metadata")
            .WithDescription("Returns metadata for a specific operation")
            .WithTags("Dynamic API")
            .Produces<object>()
            .Produces<string>(404);
    }

    private static async Task<IResult> HandleRequest(
        [FromBody] DynamicApiRequest request,
        IDynamicDispatcher dispatcher,
        ILogger<Program> logger)
    {
        try
        {
            logger.LogInformation("Processing dynamic request: {Action}", request.Action);

            if (string.IsNullOrWhiteSpace(request.Action))
            {
                return Results.BadRequest("Action is required");
            }

            // Check if operation exists
            if (!await dispatcher.OperationExistsAsync(request.Action))
            {
                logger.LogWarning("Operation not found: {Action}", request.Action);
                return Results.NotFound($"Operation '{request.Action}' not found");
            }

            // Execute the operation
            var result = await dispatcher.DispatchAsync(request.Action, request.Payload);

            logger.LogInformation("Successfully executed operation: {Action}", request.Action);
            return Results.Ok(new
            {
                success = true,
                action = request.Action,
                result = result,
                timestamp = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error processing dynamic request for action: {Action}", request.Action);
            return Results.Problem(
                title: "Operation Failed",
                detail: ex.Message,
                statusCode: 500,
                instance: $"/api?action={request.Action}"
            );
        }
    }

    private static async Task<IResult> GetAvailableOperations(
        IDynamicDispatcher dispatcher,
        ILogger<Program> logger)
    {
        try
        {
            logger.LogInformation("Retrieving available operations");

            var operations = await dispatcher.GetAvailableOperationsAsync();
            
            var operationSummaries = operations.Select(op => new
            {
                action = op.Action,
                name = op.Name,
                type = op.Type.ToString(),
                description = op.Description,
                requestType = op.RequestType.Name,
                responseType = op.ResponseType.Name,
                tags = op.Tags,
                requiresAuthentication = op.RequiresAuthentication,
                permissions = op.Permissions
            });

            return Results.Ok(new
            {
                totalOperations = operations.Count(),
                operations = operationSummaries,
                discoveredAt = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving available operations");
            return Results.Problem("Failed to retrieve operations");
        }
    }

    private static async Task<IResult> GetOperationMetadata(
        string action,
        IDynamicDispatcher dispatcher,
        ILogger<Program> logger)
    {
        try
        {
            logger.LogInformation("Retrieving metadata for operation: {Action}", action);

            var operation = await dispatcher.GetOperationMetadataAsync(action);
            
            if (operation == null)
            {
                return Results.NotFound($"Operation '{action}' not found");
            }

            var metadata = new
            {
                action = operation.Action,
                name = operation.Name,
                type = operation.Type.ToString(),
                description = operation.Description,
                handlerType = operation.HandlerType.FullName,
                requestType = new
                {
                    name = operation.RequestType.Name,
                    fullName = operation.RequestType.FullName,
                    properties = operation.RequestType.GetProperties()
                        .Select(p => new
                        {
                            name = p.Name,
                            type = p.PropertyType.Name,
                            required = !p.PropertyType.IsClass || Nullable.GetUnderlyingType(p.PropertyType) == null
                        })
                },
                responseType = new
                {
                    name = operation.ResponseType.Name,
                    fullName = operation.ResponseType.FullName
                },
                tags = operation.Tags,
                requiresAuthentication = operation.RequiresAuthentication,
                permissions = operation.Permissions,
                discoveredAt = DateTime.UtcNow
            };

            return Results.Ok(metadata);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving operation metadata for: {Action}", action);
            return Results.Problem($"Failed to retrieve metadata for operation '{action}'");
        }
    }
}

/// <summary>
/// Request DTO for the dynamic API endpoint
/// </summary>
public record DynamicApiRequest
{
    /// <summary>
    /// The action to execute (operation name)
    /// </summary>
    public string Action { get; init; } = string.Empty;
    
    /// <summary>
    /// The payload data for the operation
    /// </summary>
    public JsonElement Payload { get; init; }
    
    /// <summary>
    /// Optional metadata for the request
    /// </summary>
    public Dictionary<string, object>? Metadata { get; init; }
}