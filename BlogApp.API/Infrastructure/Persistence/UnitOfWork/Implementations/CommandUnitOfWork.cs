


namespace BlogApp.API.Infrastructure.Persistence.UnitOfWork.Implementations;

[Register(ServiceLifetime.Scoped)]
public class CommandUnitOfWork : ICommandUnitOfWork
{
    private readonly CommandDbContext _context;
    private readonly IServiceProvider _serviceProvider;
    private readonly Dictionary<Type, object> _repositories;

    public CommandUnitOfWork(CommandDbContext context, IServiceProvider serviceProvider)
    {
        _context = context;
        _serviceProvider = serviceProvider;
        _repositories = new Dictionary<Type, object>();
    }

    public ICommandRepository<T> Repository<T>() where T : BaseEntity
    {
        var type = typeof(T);
        if (!_repositories.ContainsKey(type))
        {
            var repositoryType = typeof(ICommandRepository<>).MakeGenericType(type);
            var repository = _serviceProvider.GetService(repositoryType);
            if (repository == null)
            {
                throw new InvalidOperationException($"Repository for type {type.Name} is not registered in the DI container.");
            }
            _repositories[type] = repository;
        }
        return (ICommandRepository<T>)_repositories[type];
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context?.Dispose();
    }
} 