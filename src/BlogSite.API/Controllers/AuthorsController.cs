using Microsoft.AspNetCore.Mvc;
using BlogSite.Application.DTOs;
using BlogSite.Application.Interfaces;

namespace BlogSite.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthorsController : ControllerBase
{
    private readonly IAuthorService _authorService;
    private readonly ILogger<AuthorsController> _logger;

    public AuthorsController(IAuthorService authorService, ILogger<AuthorsController> logger)
    {
        _authorService = authorService;
        _logger = logger;
    }

    /// <summary>
    /// Get all authors
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<AuthorDto>>> GetAllAuthors()
    {
        try
        {
            var authors = await _authorService.GetAllAuthorsAsync();
            return Ok(authors);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting all authors");
            return StatusCode(500, "An error occurred while processing your request");
        }
    }

    /// <summary>
    /// Get an author by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<AuthorDto>> GetAuthor(Guid id)
    {
        try
        {
            var author = await _authorService.GetAuthorByIdAsync(id);
            if (author == null)
            {
                return NotFound($"Author with ID {id} not found");
            }
            return Ok(author);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting author {AuthorId}", id);
            return StatusCode(500, "An error occurred while processing your request");
        }
    }

    /// <summary>
    /// Get an author by email
    /// </summary>
    [HttpGet("email/{email}")]
    public async Task<ActionResult<AuthorDto>> GetAuthorByEmail(string email)
    {
        try
        {
            var author = await _authorService.GetAuthorByEmailAsync(email);
            if (author == null)
            {
                return NotFound($"Author with email {email} not found");
            }
            return Ok(author);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting author by email {Email}", email);
            return StatusCode(500, "An error occurred while processing your request");
        }
    }

    /// <summary>
    /// Create a new author
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<AuthorDto>> CreateAuthor([FromBody] CreateAuthorDto createDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdAuthor = await _authorService.CreateAuthorAsync(createDto);
            return CreatedAtAction(nameof(GetAuthor), new { id = createdAuthor.Id }, createdAuthor);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid operation while creating author");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating author");
            return StatusCode(500, "An error occurred while processing your request");
        }
    }

    /// <summary>
    /// Update an author
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<AuthorDto>> UpdateAuthor(Guid id, [FromBody] UpdateAuthorDto updateDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var updatedAuthor = await _authorService.UpdateAuthorAsync(id, updateDto);
            return Ok(updatedAuthor);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Author {AuthorId} not found for update", id);
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid operation while updating author {AuthorId}", id);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating author {AuthorId}", id);
            return StatusCode(500, "An error occurred while processing your request");
        }
    }

    /// <summary>
    /// Delete an author
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAuthor(Guid id)
    {
        try
        {
            await _authorService.DeleteAuthorAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Author {AuthorId} not found for deletion", id);
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid operation while deleting author {AuthorId}", id);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting author {AuthorId}", id);
            return StatusCode(500, "An error occurred while processing your request");
        }
    }
}