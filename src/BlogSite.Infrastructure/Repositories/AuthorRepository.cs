using Microsoft.EntityFrameworkCore;
using BlogSite.Application.Interfaces;
using BlogSite.Domain.Entities;
using BlogSite.Infrastructure.Data;

namespace BlogSite.Infrastructure.Repositories;

public class AuthorRepository : Repository<Author>, IAuthorRepository
{
    public AuthorRepository(BlogDbContext context) : base(context)
    {
    }

    public override async Task<IEnumerable<Author>> GetAllAsync()
    {
        return await _dbSet
            .Include(a => a.BlogPosts)
            .OrderBy(a => a.FirstName)
            .ThenBy(a => a.LastName)
            .ToListAsync();
    }

    public override async Task<Author?> GetByIdAsync(Guid id)
    {
        return await _dbSet
            .Include(a => a.BlogPosts)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<Author?> GetByEmailAsync(string email)
    {
        return await _dbSet
            .Include(a => a.BlogPosts)
            .FirstOrDefaultAsync(a => a.Email == email);
    }
}