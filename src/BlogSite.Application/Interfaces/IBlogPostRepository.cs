using BlogSite.Domain.Entities;

namespace BlogSite.Application.Interfaces;

public interface IBlogPostRepository : IRepository<BlogPost>
{
    Task<IEnumerable<BlogPost>> GetPublishedPostsAsync();
    Task<IEnumerable<BlogPost>> GetPostsByAuthorAsync(Guid authorId);
    Task<IEnumerable<BlogPost>> GetPostsByCategoryAsync(Guid categoryId);
    Task<BlogPost?> GetPostWithCommentsAsync(Guid id);
}