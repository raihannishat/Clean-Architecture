namespace BlogApp.API.Application.Features.Comment.DTOs;

public record CommentDTO(
    int Id,
    string Content,
    string AuthorName,
    string AuthorProfileImageUrl,
    int? ParentCommentId,
    DateTime CreatedAt
)
{
    public CommentDTO() : this(0, string.Empty, string.Empty, string.Empty, null, default) {}
} 