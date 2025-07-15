namespace BlogApp.API.Application.Features.Blog.DTOs;

public record CategoryDTO(
    int Id,
    string Name,
    bool IsActive
)
{
    public CategoryDTO() : this(0, string.Empty, false) {}
} 