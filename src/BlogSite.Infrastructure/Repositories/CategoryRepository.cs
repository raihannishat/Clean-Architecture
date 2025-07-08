using Microsoft.EntityFrameworkCore;
using BlogSite.Application.Interfaces;
using BlogSite.Domain.Entities;
using BlogSite.Infrastructure.Data;

namespace BlogSite.Infrastructure.Repositories;

public class CategoryRepository : Repository<Category>, ICategoryRepository
{
    public CategoryRepository(BlogDbContext context) : base(context)
    {
    }

    public override async Task<IEnumerable<Category>> GetAllAsync()
    {
        return await _dbSet
            .Include(c => c.BlogPosts)
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    public override async Task<Category?> GetByIdAsync(Guid id)
    {
        return await _dbSet
            .Include(c => c.BlogPosts)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<Category?> GetByNameAsync(string name)
    {
        return await _dbSet
            .Include(c => c.BlogPosts)
            .FirstOrDefaultAsync(c => c.Name == name);
    }
}