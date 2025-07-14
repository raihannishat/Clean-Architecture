namespace BlogApp.API.Application.Features.Blog.DTOs;

public class BlogPostListDTO
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string AuthorName { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public IEnumerable<string> Tags { get; set; } = new List<string>();
    public int CommentCount { get; set; }
} 