namespace BlogApp.API.Application.Features.Blog.DTOs;

public record BlogPostListDTO(
    int Id,
    string Title,
    string Slug,
    string AuthorName,
    string CategoryName,
    IEnumerable<string> Tags,
    int CommentCount
)
{
    public BlogPostListDTO() : this(0, string.Empty, string.Empty, string.Empty, string.Empty, Enumerable.Empty<string>(), 0) {}
} 