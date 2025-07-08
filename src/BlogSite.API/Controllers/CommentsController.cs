using Microsoft.AspNetCore.Mvc;
using BlogSite.Application.DTOs;
using BlogSite.Application.Interfaces;

namespace BlogSite.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CommentsController : ControllerBase
{
    private readonly ICommentService _commentService;
    private readonly ILogger<CommentsController> _logger;

    public CommentsController(ICommentService commentService, ILogger<CommentsController> logger)
    {
        _commentService = commentService;
        _logger = logger;
    }

    /// <summary>
    /// Get all comments
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CommentDto>>> GetAllComments()
    {
        try
        {
            var comments = await _commentService.GetAllCommentsAsync();
            return Ok(comments);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting all comments");
            return StatusCode(500, "An error occurred while processing your request");
        }
    }

    /// <summary>
    /// Get a comment by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<CommentDto>> GetComment(Guid id)
    {
        try
        {
            var comment = await _commentService.GetCommentByIdAsync(id);
            if (comment == null)
            {
                return NotFound($"Comment with ID {id} not found");
            }
            return Ok(comment);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting comment {CommentId}", id);
            return StatusCode(500, "An error occurred while processing your request");
        }
    }

    /// <summary>
    /// Get comments by blog post
    /// </summary>
    [HttpGet("post/{blogPostId}")]
    public async Task<ActionResult<IEnumerable<CommentDto>>> GetCommentsByPost(Guid blogPostId)
    {
        try
        {
            var comments = await _commentService.GetCommentsByPostAsync(blogPostId);
            return Ok(comments);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting comments for post {BlogPostId}", blogPostId);
            return StatusCode(500, "An error occurred while processing your request");
        }
    }

    /// <summary>
    /// Get approved comments by blog post
    /// </summary>
    [HttpGet("post/{blogPostId}/approved")]
    public async Task<ActionResult<IEnumerable<CommentDto>>> GetApprovedCommentsByPost(Guid blogPostId)
    {
        try
        {
            var comments = await _commentService.GetApprovedCommentsByPostAsync(blogPostId);
            return Ok(comments);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting approved comments for post {BlogPostId}", blogPostId);
            return StatusCode(500, "An error occurred while processing your request");
        }
    }

    /// <summary>
    /// Create a new comment
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<CommentDto>> CreateComment([FromBody] CreateCommentDto createDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdComment = await _commentService.CreateCommentAsync(createDto);
            return CreatedAtAction(nameof(GetComment), new { id = createdComment.Id }, createdComment);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating comment");
            return StatusCode(500, "An error occurred while processing your request");
        }
    }

    /// <summary>
    /// Update a comment
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<CommentDto>> UpdateComment(Guid id, [FromBody] UpdateCommentDto updateDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var updatedComment = await _commentService.UpdateCommentAsync(id, updateDto);
            return Ok(updatedComment);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Comment {CommentId} not found for update", id);
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating comment {CommentId}", id);
            return StatusCode(500, "An error occurred while processing your request");
        }
    }

    /// <summary>
    /// Delete a comment
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteComment(Guid id)
    {
        try
        {
            await _commentService.DeleteCommentAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Comment {CommentId} not found for deletion", id);
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting comment {CommentId}", id);
            return StatusCode(500, "An error occurred while processing your request");
        }
    }

    /// <summary>
    /// Approve a comment
    /// </summary>
    [HttpPost("{id}/approve")]
    public async Task<ActionResult<CommentDto>> ApproveComment(Guid id)
    {
        try
        {
            var approvedComment = await _commentService.ApproveCommentAsync(id);
            return Ok(approvedComment);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Comment {CommentId} not found for approval", id);
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while approving comment {CommentId}", id);
            return StatusCode(500, "An error occurred while processing your request");
        }
    }

    /// <summary>
    /// Reject a comment (unapprove)
    /// </summary>
    [HttpPost("{id}/reject")]
    public async Task<ActionResult<CommentDto>> RejectComment(Guid id)
    {
        try
        {
            var rejectedComment = await _commentService.RejectCommentAsync(id);
            return Ok(rejectedComment);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Comment {CommentId} not found for rejection", id);
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while rejecting comment {CommentId}", id);
            return StatusCode(500, "An error occurred while processing your request");
        }
    }
}