using MediatR;
using BlogSite.Application.DTOs;
using BlogSite.Application.Commands.Authors;
using BlogSite.Application.Queries.Authors;

namespace BlogSite.API.Endpoints;

public static class AuthorEndpoints
{
    public static void MapAuthorEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/authors")
            .WithTags("Authors");

        // GET /api/authors
        group.MapGet("/", GetAllAuthors)
            .WithName("GetAllAuthors")
            .WithSummary("Get all authors")
            .WithDescription("Returns a list of all authors")
            .Produces<IEnumerable<AuthorDto>>();

        // GET /api/authors/{id}
        group.MapGet("/{id:guid}", GetAuthor)
            .WithName("GetAuthor")
            .WithSummary("Get an author by ID")
            .WithDescription("Returns an author by their unique identifier")
            .Produces<AuthorDto>()
            .Produces(404);

        // GET /api/authors/email/{email}
        group.MapGet("/email/{email}", GetAuthorByEmail)
            .WithName("GetAuthorByEmail")
            .WithSummary("Get an author by email")
            .WithDescription("Returns an author by their email address")
            .Produces<AuthorDto>()
            .Produces(404);

        // POST /api/authors
        group.MapPost("/", CreateAuthor)
            .WithName("CreateAuthor")
            .WithSummary("Create a new author")
            .WithDescription("Creates a new author with the provided information")
            .Produces<AuthorDto>(201)
            .Produces(400);

        // PUT /api/authors/{id}
        group.MapPut("/{id:guid}", UpdateAuthor)
            .WithName("UpdateAuthor")
            .WithSummary("Update an author")
            .WithDescription("Updates an existing author with the provided information")
            .Produces<AuthorDto>()
            .Produces(400)
            .Produces(404);

        // DELETE /api/authors/{id}
        group.MapDelete("/{id:guid}", DeleteAuthor)
            .WithName("DeleteAuthor")
            .WithSummary("Delete an author")
            .WithDescription("Deletes an author by their unique identifier")
            .Produces(204)
            .Produces(400)
            .Produces(404);
    }

    private static async Task<IResult> GetAllAuthors(IMediator mediator, ILogger<Program> logger)
    {
        try
        {
            var authors = await mediator.Send(new GetAllAuthorsQuery());
            return Results.Ok(authors);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while getting all authors");
            return Results.Problem("An error occurred while processing your request");
        }
    }

    private static async Task<IResult> GetAuthor(Guid id, IMediator mediator, ILogger<Program> logger)
    {
        try
        {
            var author = await mediator.Send(new GetAuthorByIdQuery(id));
            if (author == null)
            {
                return Results.NotFound($"Author with ID {id} not found");
            }
            return Results.Ok(author);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while getting author {AuthorId}", id);
            return Results.Problem("An error occurred while processing your request");
        }
    }

    private static async Task<IResult> GetAuthorByEmail(string email, IMediator mediator, ILogger<Program> logger)
    {
        try
        {
            var author = await mediator.Send(new GetAuthorByEmailQuery(email));
            if (author == null)
            {
                return Results.NotFound($"Author with email {email} not found");
            }
            return Results.Ok(author);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while getting author by email {Email}", email);
            return Results.Problem("An error occurred while processing your request");
        }
    }

    private static async Task<IResult> CreateAuthor(CreateAuthorDto createDto, IMediator mediator, ILogger<Program> logger)
    {
        try
        {
            var command = new CreateAuthorCommand(
                createDto.FirstName,
                createDto.LastName,
                createDto.Email,
                createDto.Bio
            );

            var createdAuthor = await mediator.Send(command);
            return Results.CreatedAtRoute("GetAuthor", new { id = createdAuthor.Id }, createdAuthor);
        }
        catch (InvalidOperationException ex)
        {
            logger.LogWarning(ex, "Invalid operation while creating author");
            return Results.BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while creating author");
            return Results.Problem("An error occurred while processing your request");
        }
    }

    private static async Task<IResult> UpdateAuthor(Guid id, UpdateAuthorDto updateDto, IMediator mediator, ILogger<Program> logger)
    {
        try
        {
            var command = new UpdateAuthorCommand(
                id,
                updateDto.FirstName,
                updateDto.LastName,
                updateDto.Bio
            );

            var updatedAuthor = await mediator.Send(command);
            return Results.Ok(updatedAuthor);
        }
        catch (KeyNotFoundException ex)
        {
            logger.LogWarning(ex, "Author {AuthorId} not found for update", id);
            return Results.NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            logger.LogWarning(ex, "Invalid operation while updating author {AuthorId}", id);
            return Results.BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while updating author {AuthorId}", id);
            return Results.Problem("An error occurred while processing your request");
        }
    }

    private static async Task<IResult> DeleteAuthor(Guid id, IMediator mediator, ILogger<Program> logger)
    {
        try
        {
            var result = await mediator.Send(new DeleteAuthorCommand(id));
            if (!result)
            {
                return Results.NotFound($"Author with ID {id} not found");
            }
            return Results.NoContent();
        }
        catch (InvalidOperationException ex)
        {
            logger.LogWarning(ex, "Invalid operation while deleting author {AuthorId}", id);
            return Results.BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while deleting author {AuthorId}", id);
            return Results.Problem("An error occurred while processing your request");
        }
    }
}