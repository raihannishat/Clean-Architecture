using BlogSite.Application.DTOs;
using BlogSite.Application.Interfaces;

namespace BlogSite.API.Endpoints;

public static class CommentEndpoints
{
    public static void MapCommentEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/comments")
            .WithTags("Comments");

        // GET /api/comments
        group.MapGet("/", GetAllComments)
            .WithName("GetAllComments")
            .WithSummary("Get all comments")
            .WithDescription("Returns a list of all comments")
            .Produces<IEnumerable<CommentDto>>();

        // GET /api/comments/{id}
        group.MapGet("/{id:guid}", GetComment)
            .WithName("GetComment")
            .WithSummary("Get a comment by ID")
            .WithDescription("Returns a comment by its unique identifier")
            .Produces<CommentDto>()
            .Produces(404);

        // GET /api/comments/post/{blogPostId}
        group.MapGet("/post/{blogPostId:guid}", GetCommentsByPost)
            .WithName("GetCommentsByPost")
            .WithSummary("Get comments by blog post")
            .WithDescription("Returns all comments for a specific blog post")
            .Produces<IEnumerable<CommentDto>>();

        // GET /api/comments/post/{blogPostId}/approved
        group.MapGet("/post/{blogPostId:guid}/approved", GetApprovedCommentsByPost)
            .WithName("GetApprovedCommentsByPost")
            .WithSummary("Get approved comments by blog post")
            .WithDescription("Returns all approved comments for a specific blog post")
            .Produces<IEnumerable<CommentDto>>();

        // POST /api/comments
        group.MapPost("/", CreateComment)
            .WithName("CreateComment")
            .WithSummary("Create a new comment")
            .WithDescription("Creates a new comment with the provided information")
            .Produces<CommentDto>(201)
            .Produces(400);

        // PUT /api/comments/{id}
        group.MapPut("/{id:guid}", UpdateComment)
            .WithName("UpdateComment")
            .WithSummary("Update a comment")
            .WithDescription("Updates an existing comment with the provided information")
            .Produces<CommentDto>()
            .Produces(400)
            .Produces(404);

        // DELETE /api/comments/{id}
        group.MapDelete("/{id:guid}", DeleteComment)
            .WithName("DeleteComment")
            .WithSummary("Delete a comment")
            .WithDescription("Deletes a comment by its unique identifier")
            .Produces(204)
            .Produces(404);

        // POST /api/comments/{id}/approve
        group.MapPost("/{id:guid}/approve", ApproveComment)
            .WithName("ApproveComment")
            .WithSummary("Approve a comment")
            .WithDescription("Approves a comment by its unique identifier")
            .Produces<CommentDto>()
            .Produces(404);

        // POST /api/comments/{id}/reject
        group.MapPost("/{id:guid}/reject", RejectComment)
            .WithName("RejectComment")
            .WithSummary("Reject a comment")
            .WithDescription("Rejects (unapproves) a comment by its unique identifier")
            .Produces<CommentDto>()
            .Produces(404);
    }

    private static async Task<IResult> GetAllComments(ICommentService commentService, ILogger<Program> logger)
    {
        try
        {
            var comments = await commentService.GetAllCommentsAsync();
            return Results.Ok(comments);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while getting all comments");
            return Results.Problem("An error occurred while processing your request");
        }
    }

    private static async Task<IResult> GetComment(Guid id, ICommentService commentService, ILogger<Program> logger)
    {
        try
        {
            var comment = await commentService.GetCommentByIdAsync(id);
            if (comment == null)
            {
                return Results.NotFound($"Comment with ID {id} not found");
            }
            return Results.Ok(comment);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while getting comment {CommentId}", id);
            return Results.Problem("An error occurred while processing your request");
        }
    }

    private static async Task<IResult> GetCommentsByPost(Guid blogPostId, ICommentService commentService, ILogger<Program> logger)
    {
        try
        {
            var comments = await commentService.GetCommentsByPostAsync(blogPostId);
            return Results.Ok(comments);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while getting comments for post {BlogPostId}", blogPostId);
            return Results.Problem("An error occurred while processing your request");
        }
    }

    private static async Task<IResult> GetApprovedCommentsByPost(Guid blogPostId, ICommentService commentService, ILogger<Program> logger)
    {
        try
        {
            var comments = await commentService.GetApprovedCommentsByPostAsync(blogPostId);
            return Results.Ok(comments);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while getting approved comments for post {BlogPostId}", blogPostId);
            return Results.Problem("An error occurred while processing your request");
        }
    }

    private static async Task<IResult> CreateComment(CreateCommentDto createDto, ICommentService commentService, ILogger<Program> logger)
    {
        try
        {
            var createdComment = await commentService.CreateCommentAsync(createDto);
            return Results.CreatedAtRoute("GetComment", new { id = createdComment.Id }, createdComment);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while creating comment");
            return Results.Problem("An error occurred while processing your request");
        }
    }

    private static async Task<IResult> UpdateComment(Guid id, UpdateCommentDto updateDto, ICommentService commentService, ILogger<Program> logger)
    {
        try
        {
            var updatedComment = await commentService.UpdateCommentAsync(id, updateDto);
            return Results.Ok(updatedComment);
        }
        catch (KeyNotFoundException ex)
        {
            logger.LogWarning(ex, "Comment {CommentId} not found for update", id);
            return Results.NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while updating comment {CommentId}", id);
            return Results.Problem("An error occurred while processing your request");
        }
    }

    private static async Task<IResult> DeleteComment(Guid id, ICommentService commentService, ILogger<Program> logger)
    {
        try
        {
            await commentService.DeleteCommentAsync(id);
            return Results.NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            logger.LogWarning(ex, "Comment {CommentId} not found for deletion", id);
            return Results.NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while deleting comment {CommentId}", id);
            return Results.Problem("An error occurred while processing your request");
        }
    }

    private static async Task<IResult> ApproveComment(Guid id, ICommentService commentService, ILogger<Program> logger)
    {
        try
        {
            var approvedComment = await commentService.ApproveCommentAsync(id);
            return Results.Ok(approvedComment);
        }
        catch (KeyNotFoundException ex)
        {
            logger.LogWarning(ex, "Comment {CommentId} not found for approval", id);
            return Results.NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while approving comment {CommentId}", id);
            return Results.Problem("An error occurred while processing your request");
        }
    }

    private static async Task<IResult> RejectComment(Guid id, ICommentService commentService, ILogger<Program> logger)
    {
        try
        {
            var rejectedComment = await commentService.RejectCommentAsync(id);
            return Results.Ok(rejectedComment);
        }
        catch (KeyNotFoundException ex)
        {
            logger.LogWarning(ex, "Comment {CommentId} not found for rejection", id);
            return Results.NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while rejecting comment {CommentId}", id);
            return Results.Problem("An error occurred while processing your request");
        }
    }
}