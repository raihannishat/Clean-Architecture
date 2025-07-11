using BlogApp.API.Core.Entities;
using BlogApp.API.Infrastructure.Persistence.Contexts;
using BlogApp.API.Infrastructure.Persistence.Repositories.Implementations;
using BlogApp.API.Infrastructure.Persistence.Repositories.Interfaces;
using BlogApp.API.Infrastructure.Persistence.UnitOfWork.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using AutoRegister;
using System;
using System.Collections.Generic;

namespace BlogApp.API.Infrastructure.Persistence.UnitOfWork.Implementations;

[Register(ServiceLifetime.Scoped)]
public class QueryUnitOfWork : IQueryUnitOfWork
{
    private readonly QueryDbContext _context;
    private readonly IServiceProvider _serviceProvider;
    private readonly Dictionary<Type, object> _repositories;

    public QueryUnitOfWork(QueryDbContext context, IServiceProvider serviceProvider)
    {
        _context = context;
        _serviceProvider = serviceProvider;
        _repositories = new Dictionary<Type, object>();
    }

    public IQueryRepository<T> Repository<T>() where T : BaseEntity
    {
        var type = typeof(T);
        if (!_repositories.ContainsKey(type))
        {
            var repositoryType = typeof(IQueryRepository<>).MakeGenericType(type);
            var repository = _serviceProvider.GetService(repositoryType);
            if (repository == null)
            {
                var implementationType = typeof(QueryRepository<>).MakeGenericType(type);
                repository = Activator.CreateInstance(implementationType, _context);
            }
            _repositories[type] = repository;
        }
        return (IQueryRepository<T>)_repositories[type];
    }

    public void Dispose()
    {
        _context?.Dispose();
    }
} 