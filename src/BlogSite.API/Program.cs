using BlogSite.Application.Interfaces;
using BlogSite.Application.Services;
using BlogSite.Application.Mappings;
using BlogSite.Infrastructure.Data;
using BlogSite.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using System.Reflection;
using BlogSite.API.Endpoints;
using BlogSite.Application.Dispatcher;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddDbContext<BlogDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register AutoMapper
builder.Services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfile>());

// Register repositories
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IBlogPostRepository, BlogPostRepository>();
builder.Services.AddScoped<IAuthorRepository, AuthorRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ICommentRepository, CommentRepository>();

// Register MediatR for CQRS
builder.Services.AddMediatR(cfg => {
    cfg.RegisterServicesFromAssembly(typeof(BlogSite.Application.Commands.Authors.CreateAuthorCommand).Assembly);
});

// Register Dispatcher Pattern services
builder.Services.AddDispatcher();

// Register services (keeping for backward compatibility)
builder.Services.AddScoped<IBlogPostService, BlogPostService>();
builder.Services.AddScoped<IAuthorService, AuthorService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ICommentService, CommentService>();

// Add authorization services (required for UseAuthorization)
builder.Services.AddAuthorization();

// Configure API documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "BlogSite API",
        Version = "v1",
        Description = "A comprehensive blog management API built with ASP.NET Core 8.0 following clean architecture principles using Minimal APIs.",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "BlogSite Team"
        }
    });

    // Include XML comments for better API documentation
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

// Add CORS policy for frontend applications
builder.Services.AddCors(options =>
{
    options.AddPolicy("BlogSitePolicy", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "BlogSite API v1");
        c.RoutePrefix = string.Empty; // Serve Swagger UI at the app's root
    });
}

app.UseHttpsRedirection();

app.UseCors("BlogSitePolicy");

app.UseAuthorization();

// Map minimal API endpoints
app.MapAuthorEndpoints();
app.MapBlogPostEndpoints();
app.MapCategoryEndpoints();
app.MapCommentEndpoints();

// Map dispatcher endpoint for dynamic dispatching
app.MapDispatcherEndpoints();

// Create database and apply migrations if needed
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<BlogDbContext>();
    try
    {
        dbContext.Database.EnsureCreated();
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while creating/seeding the database.");
    }
}

// Register all operations in the dispatcher
app.Services.RegisterAllOperations();

app.Run();
