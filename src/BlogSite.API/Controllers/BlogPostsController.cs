using Microsoft.AspNetCore.Mvc;
using BlogSite.Application.DTOs;
using BlogSite.Application.Interfaces;

namespace BlogSite.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BlogPostsController : ControllerBase
{
    private readonly IBlogPostService _blogPostService;
    private readonly ILogger<BlogPostsController> _logger;

    public BlogPostsController(IBlogPostService blogPostService, ILogger<BlogPostsController> logger)
    {
        _blogPostService = blogPostService;
        _logger = logger;
    }

    /// <summary>
    /// Get all blog posts
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<BlogPostDto>>> GetAllPosts()
    {
        try
        {
            var posts = await _blogPostService.GetAllPostsAsync();
            return Ok(posts);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting all blog posts");
            return StatusCode(500, "An error occurred while processing your request");
        }
    }

    /// <summary>
    /// Get all published blog posts
    /// </summary>
    [HttpGet("published")]
    public async Task<ActionResult<IEnumerable<BlogPostDto>>> GetPublishedPosts()
    {
        try
        {
            var posts = await _blogPostService.GetPublishedPostsAsync();
            return Ok(posts);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting published blog posts");
            return StatusCode(500, "An error occurred while processing your request");
        }
    }

    /// <summary>
    /// Get a blog post by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<BlogPostDto>> GetPost(Guid id)
    {
        try
        {
            var post = await _blogPostService.GetPostByIdAsync(id);
            if (post == null)
            {
                return NotFound($"Blog post with ID {id} not found");
            }
            return Ok(post);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting blog post {PostId}", id);
            return StatusCode(500, "An error occurred while processing your request");
        }
    }

    /// <summary>
    /// Get blog posts by author
    /// </summary>
    [HttpGet("author/{authorId}")]
    public async Task<ActionResult<IEnumerable<BlogPostDto>>> GetPostsByAuthor(Guid authorId)
    {
        try
        {
            var posts = await _blogPostService.GetPostsByAuthorAsync(authorId);
            return Ok(posts);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting blog posts by author {AuthorId}", authorId);
            return StatusCode(500, "An error occurred while processing your request");
        }
    }

    /// <summary>
    /// Get blog posts by category
    /// </summary>
    [HttpGet("category/{categoryId}")]
    public async Task<ActionResult<IEnumerable<BlogPostDto>>> GetPostsByCategory(Guid categoryId)
    {
        try
        {
            var posts = await _blogPostService.GetPostsByCategoryAsync(categoryId);
            return Ok(posts);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting blog posts by category {CategoryId}", categoryId);
            return StatusCode(500, "An error occurred while processing your request");
        }
    }

    /// <summary>
    /// Create a new blog post
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<BlogPostDto>> CreatePost([FromBody] CreateBlogPostDto createDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdPost = await _blogPostService.CreatePostAsync(createDto);
            return CreatedAtAction(nameof(GetPost), new { id = createdPost.Id }, createdPost);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating blog post");
            return StatusCode(500, "An error occurred while processing your request");
        }
    }

    /// <summary>
    /// Update a blog post
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<BlogPostDto>> UpdatePost(Guid id, [FromBody] UpdateBlogPostDto updateDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var updatedPost = await _blogPostService.UpdatePostAsync(id, updateDto);
            return Ok(updatedPost);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Blog post {PostId} not found for update", id);
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating blog post {PostId}", id);
            return StatusCode(500, "An error occurred while processing your request");
        }
    }

    /// <summary>
    /// Delete a blog post
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePost(Guid id)
    {
        try
        {
            await _blogPostService.DeletePostAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Blog post {PostId} not found for deletion", id);
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting blog post {PostId}", id);
            return StatusCode(500, "An error occurred while processing your request");
        }
    }

    /// <summary>
    /// Publish a blog post
    /// </summary>
    [HttpPost("{id}/publish")]
    public async Task<ActionResult<BlogPostDto>> PublishPost(Guid id)
    {
        try
        {
            var publishedPost = await _blogPostService.PublishPostAsync(id);
            return Ok(publishedPost);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Blog post {PostId} not found for publishing", id);
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while publishing blog post {PostId}", id);
            return StatusCode(500, "An error occurred while processing your request");
        }
    }

    /// <summary>
    /// Unpublish a blog post
    /// </summary>
    [HttpPost("{id}/unpublish")]
    public async Task<ActionResult<BlogPostDto>> UnpublishPost(Guid id)
    {
        try
        {
            var unpublishedPost = await _blogPostService.UnpublishPostAsync(id);
            return Ok(unpublishedPost);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Blog post {PostId} not found for unpublishing", id);
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while unpublishing blog post {PostId}", id);
            return StatusCode(500, "An error occurred while processing your request");
        }
    }
}