

namespace BlogApp.API.Infrastructure.Persistence.Repositories.Interfaces;

public interface IQueryRepository<T> where T : BaseEntity
{
    Task<T?> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>> predicate);
} 