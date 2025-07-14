using BlogApp.API.Core.Entities;
using BlogApp.API.Infrastructure.Persistence.Repositories.Interfaces;
using System;

namespace BlogApp.API.Infrastructure.Persistence.UnitOfWork.Interfaces;

public interface IQueryUnitOfWork : IDisposable
{
    IQueryRepository<T> Repository<T>() where T : BaseEntity;
} 