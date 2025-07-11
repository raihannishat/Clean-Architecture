using BlogApp.API.Core.Entities;
using BlogApp.API.Infrastructure.Persistence.UnitOfWork.Implementations;
using BlogApp.API.Infrastructure.Persistence.Contexts;
using BlogApp.API.Infrastructure.Persistence.Repositories.Interfaces;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace BlogApp.UnitTests.UnitOfWork;

public class QueryUnitOfWorkTests
{
    private readonly DbContextOptions<QueryDbContext> _options;

    public QueryUnitOfWorkTests()
    {
        _options = new DbContextOptionsBuilder<QueryDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
    }

    [Fact]
    public void Repository_WithValidType_ShouldReturnRepository()
    {
        // Arrange
        using var context = new QueryDbContext(_options);
        var unitOfWork = new QueryUnitOfWork(context);

        // Act
        var repository = unitOfWork.Repository<BlogPost>();

        // Assert
        repository.Should().NotBeNull();
        repository.Should().BeOfType<QueryRepository<BlogPost>>();
    }

    [Fact]
    public void Repository_WithDifferentTypes_ShouldReturnDifferentRepositories()
    {
        // Arrange
        using var context = new QueryDbContext(_options);
        var unitOfWork = new QueryUnitOfWork(context);

        // Act
        var blogPostRepo = unitOfWork.Repository<BlogPost>();
        var categoryRepo = unitOfWork.Repository<Category>();

        // Assert
        blogPostRepo.Should().NotBeNull();
        categoryRepo.Should().NotBeNull();
        blogPostRepo.Should().NotBeSameAs(categoryRepo);
    }

    [Fact]
    public async Task Repository_WithExistingData_ShouldReturnData()
    {
        // Arrange
        using var context = new QueryDbContext(_options);
        var unitOfWork = new QueryUnitOfWork(context);
        var repository = unitOfWork.Repository<BlogPost>();

        var blogPost = new BlogPost
        {
            Title = "Test Blog Post",
            Content = "Test content",
            Slug = "test-blog-post",
            CategoryId = 1,
            AuthorId = "user-id"
        };

        context.Set<BlogPost>().Add(blogPost);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetByIdAsync(blogPost.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Title.Should().Be("Test Blog Post");
    }

    [Fact]
    public void Repository_WithMultipleCalls_ShouldReturnSameInstance()
    {
        // Arrange
        using var context = new QueryDbContext(_options);
        var unitOfWork = new QueryUnitOfWork(context);

        // Act
        var repository1 = unitOfWork.Repository<BlogPost>();
        var repository2 = unitOfWork.Repository<BlogPost>();

        // Assert
        repository1.Should().BeSameAs(repository2);
    }

    [Fact]
    public void Dispose_ShouldDisposeContext()
    {
        // Arrange
        var context = new QueryDbContext(_options);
        var unitOfWork = new QueryUnitOfWork(context);

        // Act
        unitOfWork.Dispose();

        // Assert
        context.Database.ProviderName.Should().Be("Microsoft.EntityFrameworkCore.InMemory");
    }

    [Fact]
    public async Task DisposeAsync_ShouldDisposeContext()
    {
        // Arrange
        var context = new QueryDbContext(_options);
        var unitOfWork = new QueryUnitOfWork(context);

        // Act
        await unitOfWork.DisposeAsync();

        // Assert
        context.Database.ProviderName.Should().Be("Microsoft.EntityFrameworkCore.InMemory");
    }

    [Fact]
    public async Task Repository_WithComplexQuery_ShouldWorkCorrectly()
    {
        // Arrange
        using var context = new QueryDbContext(_options);
        var unitOfWork = new QueryUnitOfWork(context);
        var repository = unitOfWork.Repository<BlogPost>();

        var blogPosts = new List<BlogPost>
        {
            new()
            {
                Title = "Technology Blog Post",
                Content = "Technology content",
                Slug = "technology-blog-post",
                CategoryId = 1,
                AuthorId = "user-1"
            },
            new()
            {
                Title = "Travel Blog Post",
                Content = "Travel content",
                Slug = "travel-blog-post",
                CategoryId = 2,
                AuthorId = "user-2"
            }
        };

        context.Set<BlogPost>().AddRange(blogPosts);
        await context.SaveChangesAsync();

        // Act
        var technologyPosts = await repository.FindAsync(x => x.CategoryId == 1);
        var travelPosts = await repository.FindAsync(x => x.CategoryId == 2);

        // Assert
        technologyPosts.Should().HaveCount(1);
        technologyPosts.First().Title.Should().Be("Technology Blog Post");
        
        travelPosts.Should().HaveCount(1);
        travelPosts.First().Title.Should().Be("Travel Blog Post");
    }
} 