namespace BlogApp.API.DTOs;

public class BlogPostDTO
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string FeaturedImageUrl { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? PublishedAt { get; set; }
    public int ViewCount { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public string AuthorId { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public int CategoryId { get; set; }
    public List<string> Tags { get; set; } = new List<string>();
    public List<CommentDTO> Comments { get; set; } = new List<CommentDTO>();
}

public class BlogPostListDTO
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string FeaturedImageUrl { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? PublishedAt { get; set; }
    public int ViewCount { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public List<string> Tags { get; set; } = new List<string>();
    public int CommentCount { get; set; }
}

public class CreateBlogPostDTO
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public string FeaturedImageUrl { get; set; } = string.Empty;
    public int CategoryId { get; set; }
    public List<int> TagIds { get; set; } = new List<int>();
    public bool IsPublished { get; set; } = false;
}

public class UpdateBlogPostDTO
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public string FeaturedImageUrl { get; set; } = string.Empty;
    public int CategoryId { get; set; }
    public List<int> TagIds { get; set; } = new List<int>();
    public bool IsPublished { get; set; } = false;
} 