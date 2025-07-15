namespace BlogApp.API.Application.Features.Blog.DTOs;

public record TagDTO(
    int Id,
    string Name,
    bool IsActive
)
{
    public TagDTO() : this(0, string.Empty, false) {}
} 