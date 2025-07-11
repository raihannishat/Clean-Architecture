using BlogApp.API.Core.Entities;
using BlogApp.API.Infrastructure.Persistence.Repositories.Interfaces;
using System;
using System.Threading.Tasks;

namespace BlogApp.API.Infrastructure.Persistence.UnitOfWork.Interfaces;

public interface ICommandUnitOfWork : IDisposable
{
    ICommandRepository<T> Repository<T>() where T : BaseEntity;
    Task<int> SaveChangesAsync();
} 