using BlogSite.Domain.Entities;

namespace BlogSite.Application.Interfaces;

public interface IAuthorRepository : IRepository<Author>
{
    Task<Author?> GetByEmailAsync(string email);
}