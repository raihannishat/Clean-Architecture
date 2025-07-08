namespace BlogSite.Domain.Entities;

public class Comment : BaseEntity
{
    public string AuthorName { get; set; } = string.Empty;
    public string AuthorEmail { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public bool IsApproved { get; set; } = false;

    // Foreign key
    public Guid BlogPostId { get; set; }

    // Navigation property
    public BlogPost BlogPost { get; set; } = null!;
}