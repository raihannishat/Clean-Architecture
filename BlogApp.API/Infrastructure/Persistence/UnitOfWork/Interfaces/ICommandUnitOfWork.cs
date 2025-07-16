

namespace BlogApp.API.Infrastructure.Persistence.UnitOfWork.Interfaces;

public interface ICommandUnitOfWork : IDisposable
{
    ICommandRepository<T> Repository<T>() where T : BaseEntity;
    Task<int> SaveChangesAsync();
} 