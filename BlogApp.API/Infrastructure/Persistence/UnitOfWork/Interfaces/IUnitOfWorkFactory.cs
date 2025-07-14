using BlogApp.API.Infrastructure.Persistence.UnitOfWork.Interfaces;

namespace BlogApp.API.Infrastructure.Persistence.UnitOfWork.Interfaces;

public interface IUnitOfWorkFactory
{
    ICommandUnitOfWork CreateCommandUnitOfWork();
    IQueryUnitOfWork CreateQueryUnitOfWork();
} 