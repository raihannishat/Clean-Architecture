

namespace BlogApp.API.Infrastructure.Persistence.Factories;

public interface IDbContextFactory
{
    CommandDbContext CreateCommandDbContext();
    QueryDbContext CreateQueryDbContext();
} 