namespace BlogApp.API.Application.Features.Blog.DTOs;

public record BlogPostDTO(
    int Id,
    string Title,
    string Content,
    string Slug,
    string AuthorName,
    string CategoryName,
    IEnumerable<string> Tags
)
{
    public BlogPostDTO() : this(0, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, Enumerable.Empty<string>()) {}
} 