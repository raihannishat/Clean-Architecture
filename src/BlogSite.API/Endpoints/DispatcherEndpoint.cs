using System.Text.Json;
using BlogSite.Application.Dispatcher;
using Microsoft.AspNetCore.Mvc;

namespace BlogSite.API.Endpoints;

public static class DispatcherEndpoint
{
    public static void MapDispatcherEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/dispatch")
            .WithTags("Dispatcher")
            .WithDescription("Single endpoint for dynamic command and query dispatching");

        // Main dispatch endpoint
        group.MapPost("/", HandleDispatchRequest)
            .WithName("Dispatch")
            .WithSummary("Execute any command or query dynamically")
            .WithDescription("Single endpoint that can execute any registered command or query based on the request payload")
            .Produces<DispatchResult>()
            .Produces(400)
            .Produces(500);

        // Convenience endpoint with route parameters
        group.MapPost("/{operationType}/{entityType}/{action}", HandleDispatchRequestWithRoute)
            .WithName("DispatchWithRoute")
            .WithSummary("Execute command or query with route parameters")
            .WithDescription("Execute operations with type and action specified in the route")
            .Produces<DispatchResult>()
            .Produces(400)
            .Produces(500);

        // Operations discovery endpoint
        group.MapGet("/operations", GetAvailableOperations)
            .WithName("GetOperations")
            .WithSummary("Get all available operations")
            .WithDescription("Returns a list of all registered operations that can be dispatched")
            .Produces<IEnumerable<OperationSummary>>();

        // Health check endpoint for dispatcher
        group.MapGet("/health", GetDispatcherHealth)
            .WithName("DispatcherHealth")
            .WithSummary("Check dispatcher health")
            .WithDescription("Returns the health status of the dispatcher system")
            .Produces<DispatcherHealthResponse>();
    }

    private static async Task<IResult> HandleDispatchRequest(
        [FromBody] DispatchRequestDto requestDto,
        IDispatcher dispatcher,
        ILogger<Program> logger)
    {
        try
        {
            logger.LogInformation("Handling dispatch request: {OperationType}.{EntityType}.{Action}",
                requestDto.OperationType, requestDto.EntityType, requestDto.Action);

            var request = new DispatchRequest(
                requestDto.OperationType,
                requestDto.EntityType,
                requestDto.Action,
                requestDto.Payload,
                requestDto.Parameters
            );

            var result = await dispatcher.DispatchAsync(request);

            return Results.Ok(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error handling dispatch request");
            return Results.Problem("An error occurred while processing the dispatch request");
        }
    }

    private static async Task<IResult> HandleDispatchRequestWithRoute(
        string operationType,
        string entityType,
        string action,
        [FromBody] object? payload,
        [FromQuery] Dictionary<string, string>? queryParams,
        IDispatcher dispatcher,
        ILogger<Program> logger,
        HttpContext context)
    {
        try
        {
            logger.LogInformation("Handling dispatch request with route: {OperationType}.{EntityType}.{Action}",
                operationType, entityType, action);

            // Extract route parameters (like {id} from the URL)
            var routeParams = new Dictionary<string, object>();
            
            // Add query parameters
            if (queryParams != null)
            {
                foreach (var (key, value) in queryParams)
                {
                    routeParams[key] = value;
                }
            }

            // Extract route values
            foreach (var routeValue in context.Request.RouteValues)
            {
                if (routeValue.Key != "operationType" && 
                    routeValue.Key != "entityType" && 
                    routeValue.Key != "action" &&
                    routeValue.Value != null)
                {
                    routeParams[routeValue.Key] = routeValue.Value;
                }
            }

            JsonElement? payloadElement = null;
            if (payload != null)
            {
                var json = JsonSerializer.Serialize(payload);
                payloadElement = JsonSerializer.Deserialize<JsonElement>(json);
            }

            var request = new DispatchRequest(
                operationType,
                entityType,
                action,
                payloadElement,
                routeParams
            );

            var result = await dispatcher.DispatchAsync(request);

            return Results.Ok(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error handling dispatch request with route");
            return Results.Problem("An error occurred while processing the dispatch request");
        }
    }

    private static IResult GetAvailableOperations(
        IRequestTypeRegistry registry,
        ILogger<Program> logger)
    {
        try
        {
            var operations = registry.GetOperationSummaries();
            return Results.Ok(operations);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting available operations");
            return Results.Problem("An error occurred while retrieving operations");
        }
    }

    private static IResult GetDispatcherHealth(
        IRequestTypeRegistry registry,
        ILogger<Program> logger)
    {
        try
        {
            var operations = registry.GetAllOperations().ToList();
            var response = new DispatcherHealthResponse(
                IsHealthy: true,
                RegisteredOperationsCount: operations.Count,
                OperationsByType: operations
                    .GroupBy(o => o.OperationType)
                    .ToDictionary(g => g.Key, g => g.Count()),
                OperationsByEntity: operations
                    .GroupBy(o => o.EntityType)
                    .ToDictionary(g => g.Key, g => g.Count()),
                Timestamp: DateTime.UtcNow
            );

            return Results.Ok(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error checking dispatcher health");
            
            var errorResponse = new DispatcherHealthResponse(
                IsHealthy: false,
                RegisteredOperationsCount: 0,
                OperationsByType: new Dictionary<string, int>(),
                OperationsByEntity: new Dictionary<string, int>(),
                Timestamp: DateTime.UtcNow,
                ErrorMessage: ex.Message
            );

            return Results.Ok(errorResponse);
        }
    }
}

/// <summary>
/// DTO for dispatch requests via the API
/// </summary>
public record DispatchRequestDto(
    string OperationType,
    string EntityType,
    string Action,
    JsonElement? Payload = null,
    Dictionary<string, object>? Parameters = null
);

/// <summary>
/// Response for dispatcher health checks
/// </summary>
public record DispatcherHealthResponse(
    bool IsHealthy,
    int RegisteredOperationsCount,
    Dictionary<string, int> OperationsByType,
    Dictionary<string, int> OperationsByEntity,
    DateTime Timestamp,
    string? ErrorMessage = null
);