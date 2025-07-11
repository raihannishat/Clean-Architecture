using System;

namespace BlogApp.API.Core.Entities;

[DbEntity]
public class BlogPostTag : BaseEntity
{
    public int BlogPostId { get; set; }
    public virtual BlogPost BlogPost { get; set; } = null!;
    
    public int TagId { get; set; }
    public virtual Tag Tag { get; set; } = null!;
} 