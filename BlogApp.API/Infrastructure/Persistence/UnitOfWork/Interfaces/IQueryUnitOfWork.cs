

namespace BlogApp.API.Infrastructure.Persistence.UnitOfWork.Interfaces;

public interface IQueryUnitOfWork : IDisposable
{
    IQueryRepository<T> Repository<T>() where T : BaseEntity;
} 