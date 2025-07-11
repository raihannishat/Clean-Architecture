using BlogApp.API.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.API.Infrastructure.Persistence.Factories;

public interface IDbContextFactory
{
    CommandDbContext CreateCommandDbContext();
    QueryDbContext CreateQueryDbContext();
} 