using BlogSite.Application.DTOs;
using BlogSite.Application.Interfaces;

namespace BlogSite.API.Endpoints;

public static class BlogPostEndpoints
{
    public static void MapBlogPostEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/blogposts")
            .WithTags("BlogPosts");

        // GET /api/blogposts
        group.MapGet("/", GetAllPosts)
            .WithName("GetAllPosts")
            .WithSummary("Get all blog posts")
            .WithDescription("Returns a list of all blog posts")
            .Produces<IEnumerable<BlogPostDto>>();

        // GET /api/blogposts/published
        group.MapGet("/published", GetPublishedPosts)
            .WithName("GetPublishedPosts")
            .WithSummary("Get all published blog posts")
            .WithDescription("Returns a list of all published blog posts")
            .Produces<IEnumerable<BlogPostDto>>();

        // GET /api/blogposts/{id}
        group.MapGet("/{id:guid}", GetPost)
            .WithName("GetPost")
            .WithSummary("Get a blog post by ID")
            .WithDescription("Returns a blog post by its unique identifier")
            .Produces<BlogPostDto>()
            .Produces(404);

        // GET /api/blogposts/author/{authorId}
        group.MapGet("/author/{authorId:guid}", GetPostsByAuthor)
            .WithName("GetPostsByAuthor")
            .WithSummary("Get blog posts by author")
            .WithDescription("Returns all blog posts by a specific author")
            .Produces<IEnumerable<BlogPostDto>>();

        // GET /api/blogposts/category/{categoryId}
        group.MapGet("/category/{categoryId:guid}", GetPostsByCategory)
            .WithName("GetPostsByCategory")
            .WithSummary("Get blog posts by category")
            .WithDescription("Returns all blog posts in a specific category")
            .Produces<IEnumerable<BlogPostDto>>();

        // POST /api/blogposts
        group.MapPost("/", CreatePost)
            .WithName("CreatePost")
            .WithSummary("Create a new blog post")
            .WithDescription("Creates a new blog post with the provided information")
            .Produces<BlogPostDto>(201)
            .Produces(400);

        // PUT /api/blogposts/{id}
        group.MapPut("/{id:guid}", UpdatePost)
            .WithName("UpdatePost")
            .WithSummary("Update a blog post")
            .WithDescription("Updates an existing blog post with the provided information")
            .Produces<BlogPostDto>()
            .Produces(400)
            .Produces(404);

        // DELETE /api/blogposts/{id}
        group.MapDelete("/{id:guid}", DeletePost)
            .WithName("DeletePost")
            .WithSummary("Delete a blog post")
            .WithDescription("Deletes a blog post by its unique identifier")
            .Produces(204)
            .Produces(404);

        // POST /api/blogposts/{id}/publish
        group.MapPost("/{id:guid}/publish", PublishPost)
            .WithName("PublishPost")
            .WithSummary("Publish a blog post")
            .WithDescription("Publishes a blog post by its unique identifier")
            .Produces<BlogPostDto>()
            .Produces(404);

        // POST /api/blogposts/{id}/unpublish
        group.MapPost("/{id:guid}/unpublish", UnpublishPost)
            .WithName("UnpublishPost")
            .WithSummary("Unpublish a blog post")
            .WithDescription("Unpublishes a blog post by its unique identifier")
            .Produces<BlogPostDto>()
            .Produces(404);
    }

    private static async Task<IResult> GetAllPosts(IBlogPostService blogPostService, ILogger<Program> logger)
    {
        try
        {
            var posts = await blogPostService.GetAllPostsAsync();
            return Results.Ok(posts);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while getting all blog posts");
            return Results.Problem("An error occurred while processing your request");
        }
    }

    private static async Task<IResult> GetPublishedPosts(IBlogPostService blogPostService, ILogger<Program> logger)
    {
        try
        {
            var posts = await blogPostService.GetPublishedPostsAsync();
            return Results.Ok(posts);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while getting published blog posts");
            return Results.Problem("An error occurred while processing your request");
        }
    }

    private static async Task<IResult> GetPost(Guid id, IBlogPostService blogPostService, ILogger<Program> logger)
    {
        try
        {
            var post = await blogPostService.GetPostByIdAsync(id);
            if (post == null)
            {
                return Results.NotFound($"Blog post with ID {id} not found");
            }
            return Results.Ok(post);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while getting blog post {PostId}", id);
            return Results.Problem("An error occurred while processing your request");
        }
    }

    private static async Task<IResult> GetPostsByAuthor(Guid authorId, IBlogPostService blogPostService, ILogger<Program> logger)
    {
        try
        {
            var posts = await blogPostService.GetPostsByAuthorAsync(authorId);
            return Results.Ok(posts);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while getting blog posts by author {AuthorId}", authorId);
            return Results.Problem("An error occurred while processing your request");
        }
    }

    private static async Task<IResult> GetPostsByCategory(Guid categoryId, IBlogPostService blogPostService, ILogger<Program> logger)
    {
        try
        {
            var posts = await blogPostService.GetPostsByCategoryAsync(categoryId);
            return Results.Ok(posts);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while getting blog posts by category {CategoryId}", categoryId);
            return Results.Problem("An error occurred while processing your request");
        }
    }

    private static async Task<IResult> CreatePost(CreateBlogPostDto createDto, IBlogPostService blogPostService, ILogger<Program> logger)
    {
        try
        {
            var createdPost = await blogPostService.CreatePostAsync(createDto);
            return Results.CreatedAtRoute("GetPost", new { id = createdPost.Id }, createdPost);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while creating blog post");
            return Results.Problem("An error occurred while processing your request");
        }
    }

    private static async Task<IResult> UpdatePost(Guid id, UpdateBlogPostDto updateDto, IBlogPostService blogPostService, ILogger<Program> logger)
    {
        try
        {
            var updatedPost = await blogPostService.UpdatePostAsync(id, updateDto);
            return Results.Ok(updatedPost);
        }
        catch (KeyNotFoundException ex)
        {
            logger.LogWarning(ex, "Blog post {PostId} not found for update", id);
            return Results.NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while updating blog post {PostId}", id);
            return Results.Problem("An error occurred while processing your request");
        }
    }

    private static async Task<IResult> DeletePost(Guid id, IBlogPostService blogPostService, ILogger<Program> logger)
    {
        try
        {
            await blogPostService.DeletePostAsync(id);
            return Results.NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            logger.LogWarning(ex, "Blog post {PostId} not found for deletion", id);
            return Results.NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while deleting blog post {PostId}", id);
            return Results.Problem("An error occurred while processing your request");
        }
    }

    private static async Task<IResult> PublishPost(Guid id, IBlogPostService blogPostService, ILogger<Program> logger)
    {
        try
        {
            var publishedPost = await blogPostService.PublishPostAsync(id);
            return Results.Ok(publishedPost);
        }
        catch (KeyNotFoundException ex)
        {
            logger.LogWarning(ex, "Blog post {PostId} not found for publishing", id);
            return Results.NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while publishing blog post {PostId}", id);
            return Results.Problem("An error occurred while processing your request");
        }
    }

    private static async Task<IResult> UnpublishPost(Guid id, IBlogPostService blogPostService, ILogger<Program> logger)
    {
        try
        {
            var unpublishedPost = await blogPostService.UnpublishPostAsync(id);
            return Results.Ok(unpublishedPost);
        }
        catch (KeyNotFoundException ex)
        {
            logger.LogWarning(ex, "Blog post {PostId} not found for unpublishing", id);
            return Results.NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while unpublishing blog post {PostId}", id);
            return Results.Problem("An error occurred while processing your request");
        }
    }
}