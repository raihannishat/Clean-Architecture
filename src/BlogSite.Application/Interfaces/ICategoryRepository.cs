using BlogSite.Domain.Entities;

namespace BlogSite.Application.Interfaces;

public interface ICategoryRepository : IRepository<Category>
{
    Task<Category?> GetByNameAsync(string name);
}