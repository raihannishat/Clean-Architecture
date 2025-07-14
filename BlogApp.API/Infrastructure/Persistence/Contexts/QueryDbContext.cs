using Microsoft.EntityFrameworkCore;
using BlogApp.API.Core.Entities;
using System.Reflection;

namespace BlogApp.API.Infrastructure.Persistence.Contexts;

public class QueryDbContext : DbContext
{
    public QueryDbContext(DbContextOptions<QueryDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var entityTypes = typeof(BaseEntity).Assembly
            .GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && t.GetCustomAttribute<DbEntityAttribute>() != null);

        foreach (var type in entityTypes)
        {
            modelBuilder.Entity(type);
        }

        base.OnModelCreating(modelBuilder);
    }
} 