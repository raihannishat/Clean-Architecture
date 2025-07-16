

namespace BlogApp.API.Infrastructure.Persistence.UnitOfWork.Implementations;

[Register(ServiceLifetime.Scoped)]
public class UnitOfWorkFactory : IUnitOfWorkFactory
{
    private readonly IDbContextFactory _dbContextFactory;
    private readonly IServiceProvider _serviceProvider;

    public UnitOfWorkFactory(IDbContextFactory dbContextFactory, IServiceProvider serviceProvider)
    {
        _dbContextFactory = dbContextFactory;
        _serviceProvider = serviceProvider;
    }

    public ICommandUnitOfWork CreateCommandUnitOfWork()
    {
        var commandContext = _dbContextFactory.CreateCommandDbContext();
        return new CommandUnitOfWork(commandContext, _serviceProvider);
    }

    public IQueryUnitOfWork CreateQueryUnitOfWork()
    {
        var queryContext = _dbContextFactory.CreateQueryDbContext();
        return new QueryUnitOfWork(queryContext, _serviceProvider);
    }
} 