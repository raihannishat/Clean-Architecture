namespace BlogApp.API.DTOs;

public class CommentDTO
{
    public int Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public string AuthorId { get; set; } = string.Empty;
    public string AuthorProfileImageUrl { get; set; } = string.Empty;
    public int? ParentCommentId { get; set; }
    public List<CommentDTO> Replies { get; set; } = new List<CommentDTO>();
}

public class CreateCommentDTO
{
    public string Content { get; set; } = string.Empty;
    public int BlogPostId { get; set; }
    public int? ParentCommentId { get; set; }
}

public class UpdateCommentDTO
{
    public string Content { get; set; } = string.Empty;
} 