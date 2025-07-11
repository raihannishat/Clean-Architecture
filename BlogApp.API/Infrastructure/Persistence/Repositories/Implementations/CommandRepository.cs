using BlogApp.API.Core.Entities;
using BlogApp.API.Infrastructure.Persistence.Contexts;
using BlogApp.API.Infrastructure.Persistence.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoRegister;

namespace BlogApp.API.Infrastructure.Persistence.Repositories.Implementations;

[Register(ServiceLifetime.Scoped)]
public class CommandRepository<T> : ICommandRepository<T> where T : BaseEntity
{
    private readonly CommandDbContext _context;
    private readonly DbSet<T> _dbSet;

    public CommandRepository(CommandDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public virtual async Task<T?> GetByIdAsync(int id) => await _dbSet.FindAsync(id);
    public virtual async Task<IEnumerable<T>> GetAllAsync() => await _dbSet.ToListAsync();
    public virtual async Task AddAsync(T entity) => await _dbSet.AddAsync(entity);
    public virtual void Update(T entity) => _dbSet.Update(entity);
    public virtual void Delete(T entity) => _dbSet.Remove(entity);
} 