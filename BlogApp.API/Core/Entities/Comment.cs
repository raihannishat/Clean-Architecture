using System;
using System.ComponentModel.DataAnnotations;

namespace BlogApp.API.Core.Entities;

[DbEntity]
public class Comment : BaseEntity
{
    public string Content { get; set; } = string.Empty;
    public bool IsApproved { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    public string AuthorId { get; set; } = string.Empty;
    public virtual ApplicationUser Author { get; set; } = null!;
    
    public int BlogPostId { get; set; }
    public virtual BlogPost BlogPost { get; set; } = null!;
    
    public int? ParentCommentId { get; set; }
    public virtual Comment? ParentComment { get; set; }
    public virtual ICollection<Comment> Replies { get; set; } = new List<Comment>();
} 