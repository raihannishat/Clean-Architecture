using Microsoft.EntityFrameworkCore;
using BlogSite.Application.Interfaces;
using BlogSite.Domain.Entities;
using BlogSite.Infrastructure.Data;

namespace BlogSite.Infrastructure.Repositories;

public class BlogPostRepository : Repository<BlogPost>, IBlogPostRepository
{
    public BlogPostRepository(BlogDbContext context) : base(context)
    {
    }

    public override async Task<IEnumerable<BlogPost>> GetAllAsync()
    {
        return await _dbSet
            .Include(p => p.Author)
            .Include(p => p.Category)
            .Include(p => p.Comments)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    public override async Task<BlogPost?> GetByIdAsync(Guid id)
    {
        return await _dbSet
            .Include(p => p.Author)
            .Include(p => p.Category)
            .Include(p => p.Comments)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<IEnumerable<BlogPost>> GetPublishedPostsAsync()
    {
        return await _dbSet
            .Include(p => p.Author)
            .Include(p => p.Category)
            .Where(p => p.IsPublished)
            .OrderByDescending(p => p.PublishedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<BlogPost>> GetPostsByAuthorAsync(Guid authorId)
    {
        return await _dbSet
            .Include(p => p.Author)
            .Include(p => p.Category)
            .Include(p => p.Comments)
            .Where(p => p.AuthorId == authorId)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<BlogPost>> GetPostsByCategoryAsync(Guid categoryId)
    {
        return await _dbSet
            .Include(p => p.Author)
            .Include(p => p.Category)
            .Include(p => p.Comments)
            .Where(p => p.CategoryId == categoryId)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    public async Task<BlogPost?> GetPostWithCommentsAsync(Guid id)
    {
        return await _dbSet
            .Include(p => p.Author)
            .Include(p => p.Category)
            .Include(p => p.Comments.Where(c => c.IsApproved))
            .FirstOrDefaultAsync(p => p.Id == id);
    }
}