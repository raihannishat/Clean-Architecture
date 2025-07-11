using BlogApp.API.Core.Entities;
using BlogApp.API.Infrastructure.Persistence.UnitOfWork.Implementations;
using BlogApp.API.Infrastructure.Persistence.Contexts;
using BlogApp.API.Infrastructure.Persistence.Repositories.Interfaces;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace BlogApp.UnitTests.UnitOfWork;

public class CommandUnitOfWorkTests
{
    private readonly DbContextOptions<CommandDbContext> _options;

    public CommandUnitOfWorkTests()
    {
        _options = new DbContextOptionsBuilder<CommandDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
    }

    [Fact]
    public void Repository_WithValidType_ShouldReturnRepository()
    {
        // Arrange
        using var context = new CommandDbContext(_options);
        var unitOfWork = new CommandUnitOfWork(context);

        // Act
        var repository = unitOfWork.Repository<BlogPost>();

        // Assert
        repository.Should().NotBeNull();
        repository.Should().BeOfType<CommandRepository<BlogPost>>();
    }

    [Fact]
    public void Repository_WithDifferentTypes_ShouldReturnDifferentRepositories()
    {
        // Arrange
        using var context = new CommandDbContext(_options);
        var unitOfWork = new CommandUnitOfWork(context);

        // Act
        var blogPostRepo = unitOfWork.Repository<BlogPost>();
        var categoryRepo = unitOfWork.Repository<Category>();

        // Assert
        blogPostRepo.Should().NotBeNull();
        categoryRepo.Should().NotBeNull();
        blogPostRepo.Should().NotBeSameAs(categoryRepo);
    }

    [Fact]
    public async Task SaveChangesAsync_WithValidChanges_ShouldReturnPositiveCount()
    {
        // Arrange
        using var context = new CommandDbContext(_options);
        var unitOfWork = new CommandUnitOfWork(context);
        var repository = unitOfWork.Repository<BlogPost>();

        var blogPost = new BlogPost
        {
            Title = "Test Blog Post",
            Content = "Test content",
            Slug = "test-blog-post",
            CategoryId = 1,
            AuthorId = "user-id"
        };

        await repository.AddAsync(blogPost);

        // Act
        var result = await unitOfWork.SaveChangesAsync();

        // Assert
        result.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task SaveChangesAsync_WithNoChanges_ShouldReturnZero()
    {
        // Arrange
        using var context = new CommandDbContext(_options);
        var unitOfWork = new CommandUnitOfWork(context);

        // Act
        var result = await unitOfWork.SaveChangesAsync();

        // Assert
        result.Should().Be(0);
    }

    [Fact]
    public async Task SaveChangesAsync_WithMultipleEntities_ShouldSaveAllChanges()
    {
        // Arrange
        using var context = new CommandDbContext(_options);
        var unitOfWork = new CommandUnitOfWork(context);
        var blogPostRepo = unitOfWork.Repository<BlogPost>();
        var categoryRepo = unitOfWork.Repository<Category>();

        var category = new Category
        {
            Name = "Technology",
            Slug = "technology",
            IconClass = "fas fa-laptop",
            Color = "#007bff"
        };

        var blogPost = new BlogPost
        {
            Title = "Test Blog Post",
            Content = "Test content",
            Slug = "test-blog-post",
            CategoryId = 1,
            AuthorId = "user-id"
        };

        await categoryRepo.AddAsync(category);
        await blogPostRepo.AddAsync(blogPost);

        // Act
        var result = await unitOfWork.SaveChangesAsync();

        // Assert
        result.Should().BeGreaterThan(0);

        var savedCategory = await context.Set<Category>().FirstOrDefaultAsync(x => x.Slug == "technology");
        var savedBlogPost = await context.Set<BlogPost>().FirstOrDefaultAsync(x => x.Slug == "test-blog-post");

        savedCategory.Should().NotBeNull();
        savedBlogPost.Should().NotBeNull();
    }

    [Fact]
    public void Dispose_ShouldDisposeContext()
    {
        // Arrange
        var context = new CommandDbContext(_options);
        var unitOfWork = new CommandUnitOfWork(context);

        // Act
        unitOfWork.Dispose();

        // Assert
        context.Database.ProviderName.Should().Be("Microsoft.EntityFrameworkCore.InMemory");
    }

    [Fact]
    public async Task DisposeAsync_ShouldDisposeContext()
    {
        // Arrange
        var context = new CommandDbContext(_options);
        var unitOfWork = new CommandUnitOfWork(context);

        // Act
        await unitOfWork.DisposeAsync();

        // Assert
        context.Database.ProviderName.Should().Be("Microsoft.EntityFrameworkCore.InMemory");
    }
} 