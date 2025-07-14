using System;
using System.ComponentModel.DataAnnotations;

namespace BlogApp.API.Core.Entities;

[DbEntity]
public class Category : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string IconClass { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    public virtual ICollection<BlogPost> BlogPosts { get; set; } = new List<BlogPost>();
} 