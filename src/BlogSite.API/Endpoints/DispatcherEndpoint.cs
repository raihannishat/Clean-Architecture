using System.Text.Json;
using BlogSite.Application.Dispatcher;
using Microsoft.AspNetCore.Mvc;

namespace BlogSite.API.Endpoints;

public static class DispatcherEndpoint
{
    public static void MapDispatcherEndpoints(this IEndpointRouteBuilder endpoints)
    {
        // Single dynamic endpoint that handles all operations
        endpoints.MapPost("/api", HandleRequest)
            .WithName("API")
            .WithSummary("Main API endpoint - Execute any operation dynamically")
            .WithDescription("Single universal endpoint that can execute any registered command or query based on the 'action' parameter")
            .WithTags("Main API")
            .Produces<object>()
            .Produces(400)
            .Produces(500);
    }

    private static async Task<IResult> HandleRequest(
        [FromBody] ApiRequest request,
        IDispatcher dispatcher,
        ILogger<Program> logger)
    {
        try
        {
            logger.LogInformation("Handling request: {Action}", request.Action);

            var dispatchRequest = new DispatchRequest("unknown", request.Action, request.Payload);
            var result = await dispatcher.DispatchAsync(dispatchRequest);

            return Results.Ok(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error handling request for action: {Action}", request.Action);
            return Results.Problem(ex.Message);
        }
    }
}

/// <summary>
/// Request DTO for the single API endpoint
/// </summary>
public record ApiRequest(
    string Action,
    JsonElement Payload
);