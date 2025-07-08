using Microsoft.EntityFrameworkCore;
using BlogSite.Application.Interfaces;
using BlogSite.Domain.Entities;
using BlogSite.Infrastructure.Data;

namespace BlogSite.Infrastructure.Repositories;

public class CommentRepository : Repository<Comment>, ICommentRepository
{
    public CommentRepository(BlogDbContext context) : base(context)
    {
    }

    public override async Task<IEnumerable<Comment>> GetAllAsync()
    {
        return await _dbSet
            .Include(c => c.BlogPost)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();
    }

    public override async Task<Comment?> GetByIdAsync(Guid id)
    {
        return await _dbSet
            .Include(c => c.BlogPost)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<IEnumerable<Comment>> GetCommentsByPostAsync(Guid postId)
    {
        return await _dbSet
            .Include(c => c.BlogPost)
            .Where(c => c.BlogPostId == postId)
            .OrderBy(c => c.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Comment>> GetApprovedCommentsAsync(Guid postId)
    {
        return await _dbSet
            .Include(c => c.BlogPost)
            .Where(c => c.BlogPostId == postId && c.IsApproved)
            .OrderBy(c => c.CreatedAt)
            .ToListAsync();
    }
}