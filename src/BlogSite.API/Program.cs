using System.Reflection;
using AutoMapper;
using BlogSite.Application.Core.Dynamic;
using BlogSite.Application.Interfaces;
using BlogSite.Application.Mappings;
using BlogSite.Application.Services;
using BlogSite.Application.Configuration;
using BlogSite.Infrastructure.Data;
using BlogSite.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using BlogSite.API.Endpoints;

var builder = WebApplication.CreateBuilder(args);

// === DYNAMIC CONFIGURATION SETUP ===
// No hardcoded values, everything discovered dynamically

// Add core services
builder.Services.AddLogging();

// Auto-detect and configure database
ConfigureDatabaseDynamically(builder.Services);

// Register AutoMapper dynamically
ConfigureAutoMapperDynamically(builder.Services);

// Register repositories dynamically
RegisterRepositoriesDynamically(builder.Services);

// Register MediatR for CQRS dynamically
RegisterMediatRDynamically(builder.Services);

// Add the dynamic dispatcher
builder.Services.AddDynamicDispatcher();

// Register services dynamically
RegisterServicesDynamically(builder.Services);

// Configure application settings
ConfigureApplicationSettings(builder.Services, builder.Configuration);

// Add HTTP context accessor
builder.Services.AddHttpContextAccessor();

// Add authorization services
builder.Services.AddAuthorization();

// Configure API documentation dynamically
ConfigureApiDocumentationDynamically(builder.Services);

// Add CORS dynamically
ConfigureCorsDynamically(builder.Services);

var app = builder.Build();

// === DYNAMIC PIPELINE CONFIGURATION ===

// Configure the HTTP request pipeline dynamically
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "BlogSite Dynamic API v1");
        c.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();
app.UseCors("DynamicCorsPolicy");
app.UseAuthorization();

// Map the completely dynamic API endpoint
app.MapDynamicApiEndpoint();

// Initialize database dynamically
await InitializeDatabaseDynamicallyAsync(app.Services);

// Discover and register all operations dynamically
await DiscoverAndRegisterOperationsAsync(app.Services);

app.Run();

// === DYNAMIC CONFIGURATION METHODS ===

static void ConfigureDatabaseDynamically(IServiceCollection services)
{
    // Auto-detect database file or create in-memory
    var dbPath = FindDatabaseFile() ?? "BlogSite.db";
    var connectionString = $"Data Source={dbPath}";
    
    services.AddDbContext<BlogDbContext>(options =>
        options.UseSqlite(connectionString));
}

static void ConfigureAutoMapperDynamically(IServiceCollection services)
{
    // Discover all mapping profiles dynamically
    var assemblies = AppDomain.CurrentDomain.GetAssemblies()
        .Where(a => !a.IsDynamic && a.FullName?.Contains("BlogSite") == true)
        .ToArray();

    if (assemblies.Any())
    {
        services.AddAutoMapper(cfg => 
        {
            cfg.AddMaps(assemblies);
        });
    }
}

static void RegisterRepositoriesDynamically(IServiceCollection services)
{
    // Register generic repository
    services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
    
    // Auto-discover and register specific repositories
    var repositoryTypes = AppDomain.CurrentDomain.GetAssemblies()
        .Where(a => !a.IsDynamic && a.FullName?.Contains("BlogSite") == true)
        .SelectMany(a => a.GetTypes())
        .Where(t => t.IsClass && !t.IsAbstract && t.Name.EndsWith("Repository"))
        .ToList();

    foreach (var repoType in repositoryTypes)
    {
        var interfaces = repoType.GetInterfaces()
            .Where(i => i != typeof(IRepository<>) && i.Name.StartsWith("I"));
        
        foreach (var interfaceType in interfaces)
        {
            services.AddScoped(interfaceType, repoType);
        }
    }
}

static void RegisterMediatRDynamically(IServiceCollection services)
{
    // Auto-discover assemblies with MediatR handlers
    var assemblies = AppDomain.CurrentDomain.GetAssemblies()
        .Where(a => !a.IsDynamic && a.FullName?.Contains("BlogSite") == true)
        .ToArray();

    if (assemblies.Any())
    {
        services.AddMediatR(cfg => {
            foreach (var assembly in assemblies)
            {
                cfg.RegisterServicesFromAssembly(assembly);
            }
        });
    }
}

static void RegisterServicesDynamically(IServiceCollection services)
{
    // Auto-discover and register all service classes
    var serviceTypes = AppDomain.CurrentDomain.GetAssemblies()
        .Where(a => !a.IsDynamic && a.FullName?.Contains("BlogSite") == true)
        .SelectMany(a => a.GetTypes())
        .Where(t => t.IsClass && !t.IsAbstract && t.Name.EndsWith("Service"))
        .ToList();

    foreach (var serviceType in serviceTypes)
    {
        // Register the service itself
        services.AddScoped(serviceType);
        
        // Register service interfaces
        var interfaces = serviceType.GetInterfaces()
            .Where(i => i.Name.StartsWith("I") && i.Name.EndsWith("Service"));
        
        foreach (var interfaceType in interfaces)
        {
            services.AddScoped(interfaceType, serviceType);
        }
    }
}

static void ConfigureApiDocumentationDynamically(IServiceCollection services)
{
    services.AddEndpointsApiExplorer();
    services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
        {
            Title = "BlogSite Dynamic API",
            Version = "v1",
            Description = "A completely dynamic API that discovers and executes operations without hardcoded configuration",
            Contact = new Microsoft.OpenApi.Models.OpenApiContact
            {
                Name = "Dynamic BlogSite Team"
            }
        });

        // Auto-discover and include XML comments
        var xmlFiles = Directory.GetFiles(AppContext.BaseDirectory, "*.xml");
        foreach (var xmlFile in xmlFiles)
        {
            c.IncludeXmlComments(xmlFile);
        }
    });
}

static void ConfigureCorsDynamically(IServiceCollection services)
{
    services.AddCors(options =>
    {
        options.AddPolicy("DynamicCorsPolicy", policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
    });
}

static void ConfigureApplicationSettings(IServiceCollection services, IConfiguration configuration)
{
    // Configure entity discovery options
    services.Configure<EntityDiscoveryOptions>(
        configuration.GetSection(EntityDiscoveryOptions.SectionName));
}

static string? FindDatabaseFile()
{
    // Look for existing database files in common locations
    var searchPaths = new[]
    {
        "BlogSite.db",
        "blogsite.db",
        "database.db",
        Path.Combine("Data", "BlogSite.db"),
        Path.Combine("App_Data", "BlogSite.db")
    };

    foreach (var path in searchPaths)
    {
        if (File.Exists(path))
        {
            return path;
        }
    }

    return null;
}

static async Task InitializeDatabaseDynamicallyAsync(IServiceProvider services)
{
    using var scope = services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<BlogDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    try
    {
        await dbContext.Database.EnsureCreatedAsync();
        logger.LogInformation("Database initialized successfully");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error initializing database");
    }
}

static async Task DiscoverAndRegisterOperationsAsync(IServiceProvider services)
{
    using var scope = services.CreateScope();
    var discoveryService = scope.ServiceProvider.GetRequiredService<IOperationDiscoveryService>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    try
    {
        var operations = await discoveryService.DiscoverOperationsAsync();
        logger.LogInformation("Discovered {Count} operations dynamically", operations.Count());
        
        foreach (var operation in operations)
        {
            logger.LogDebug("Discovered operation: {Action} ({Type})", operation.Action, operation.Type);
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error discovering operations");
    }
}
