using System;
using System.ComponentModel.DataAnnotations;

namespace BlogApp.API.Core.Entities;

[DbEntity]
public class BlogPost : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public string FeaturedImageUrl { get; set; } = string.Empty;
    public bool IsPublished { get; set; }
    public DateTime? PublishedAt { get; set; }
    public int ViewCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    public string AuthorId { get; set; } = string.Empty;
    public virtual ApplicationUser Author { get; set; } = null!;
    
    public int CategoryId { get; set; }
    public virtual Category Category { get; set; } = null!;
    
    public virtual ICollection<BlogPostTag> BlogPostTags { get; set; } = new List<BlogPostTag>();
    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
} 