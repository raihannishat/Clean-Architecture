using BlogApp.API.Core.Entities;
using BlogApp.API.Infrastructure.Persistence.Contexts;
using BlogApp.API.Infrastructure.Persistence.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoRegister;

namespace BlogApp.API.Infrastructure.Persistence.Repositories.Implementations;

[Register(ServiceLifetime.Scoped)]
public class QueryRepository<T> : IQueryRepository<T> where T : BaseEntity
{
    private readonly QueryDbContext _context;
    private readonly DbSet<T> _dbSet;

    public QueryRepository(QueryDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public virtual async Task<T?> GetByIdAsync(int id) => await _dbSet.FindAsync(id);
    public virtual async Task<IEnumerable<T>> GetAllAsync() => await _dbSet.ToListAsync();
    public virtual async Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>> predicate) => await _dbSet.Where(predicate).ToListAsync();
} 