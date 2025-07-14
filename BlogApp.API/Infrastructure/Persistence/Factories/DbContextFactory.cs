using BlogApp.API.Infrastructure.Persistence.Contexts;
using BlogApp.API.Infrastructure.Persistence.Factories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using AutoRegister;

namespace BlogApp.API.Infrastructure.Persistence.Factories;

[Register(ServiceLifetime.Scoped)]
public class DbContextFactory : IDbContextFactory
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IConfiguration _configuration;

    public DbContextFactory(IServiceProvider serviceProvider, IConfiguration configuration)
    {
        _serviceProvider = serviceProvider;
        _configuration = configuration;
    }

    public CommandDbContext CreateCommandDbContext()
    {
        var optionsBuilder = new DbContextOptionsBuilder<CommandDbContext>();
        var connectionString = _configuration.GetConnectionString("PostgreSQLConnection");
        
        optionsBuilder.UseNpgsql(connectionString);
        
        return new CommandDbContext(optionsBuilder.Options);
    }

    public QueryDbContext CreateQueryDbContext()
    {
        var optionsBuilder = new DbContextOptionsBuilder<QueryDbContext>();
        var connectionString = _configuration.GetConnectionString("MongoDBConnection");
        
        optionsBuilder.UseMongoDB(connectionString, "BlogApp");
        
        return new QueryDbContext(optionsBuilder.Options);
    }
} 