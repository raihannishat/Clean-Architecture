using BlogApp.API.Core.Entities;
using BlogApp.API.Infrastructure.Persistence.Repositories.Implementations;
using BlogApp.API.Infrastructure.Persistence.Contexts;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace BlogApp.UnitTests.Repositories;

public class QueryRepositoryTests
{
    private readonly DbContextOptions<QueryDbContext> _options;

    public QueryRepositoryTests()
    {
        _options = new DbContextOptionsBuilder<QueryDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
    }

    [Fact]
    public async Task GetByIdAsync_WithExistingId_ShouldReturnEntity()
    {
        // Arrange
        using var context = new QueryDbContext(_options);
        var repository = new QueryRepository<BlogPost>(context);
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
    public async Task GetByIdAsync_WithNonExistentId_ShouldReturnNull()
    {
        // Arrange
        using var context = new QueryDbContext(_options);
        var repository = new QueryRepository<BlogPost>(context);

        // Act
        var result = await repository.GetByIdAsync(999);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAllAsync_WithEntities_ShouldReturnAllEntities()
    {
        // Arrange
        using var context = new QueryDbContext(_options);
        var repository = new QueryRepository<BlogPost>(context);
        var blogPosts = new List<BlogPost>
        {
            new()
            {
                Title = "First Blog Post",
                Content = "First content",
                Slug = "first-blog-post",
                CategoryId = 1,
                AuthorId = "user-1"
            },
            new()
            {
                Title = "Second Blog Post",
                Content = "Second content",
                Slug = "second-blog-post",
                CategoryId = 1,
                AuthorId = "user-2"
            }
        };

        context.Set<BlogPost>().AddRange(blogPosts);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetAllAsync();

        // Assert
        result.Should().NotBeNull();
        result.Count.Should().Be(2);
        result.Should().Contain(x => x.Title == "First Blog Post");
        result.Should().Contain(x => x.Title == "Second Blog Post");
    }

    [Fact]
    public async Task GetAllAsync_WithNoEntities_ShouldReturnEmptyList()
    {
        // Arrange
        using var context = new QueryDbContext(_options);
        var repository = new QueryRepository<BlogPost>(context);

        // Act
        var result = await repository.GetAllAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task FindAsync_WithMatchingPredicate_ShouldReturnMatchingEntities()
    {
        // Arrange
        using var context = new QueryDbContext(_options);
        var repository = new QueryRepository<BlogPost>(context);
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
        var result = await repository.FindAsync(x => x.CategoryId == 1);

        // Assert
        result.Should().NotBeNull();
        result.Count.Should().Be(1);
        result.First().Title.Should().Be("Technology Blog Post");
    }

    [Fact]
    public async Task FindAsync_WithNoMatchingPredicate_ShouldReturnEmptyList()
    {
        // Arrange
        using var context = new QueryDbContext(_options);
        var repository = new QueryRepository<BlogPost>(context);
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
        var result = await repository.FindAsync(x => x.CategoryId == 999);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetFirstOrDefaultAsync_WithMatchingPredicate_ShouldReturnFirstMatch()
    {
        // Arrange
        using var context = new QueryDbContext(_options);
        var repository = new QueryRepository<BlogPost>(context);
        var blogPosts = new List<BlogPost>
        {
            new()
            {
                Title = "First Blog Post",
                Content = "First content",
                Slug = "first-blog-post",
                CategoryId = 1,
                AuthorId = "user-1"
            },
            new()
            {
                Title = "Second Blog Post",
                Content = "Second content",
                Slug = "second-blog-post",
                CategoryId = 1,
                AuthorId = "user-2"
            }
        };

        context.Set<BlogPost>().AddRange(blogPosts);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetFirstOrDefaultAsync(x => x.CategoryId == 1);

        // Assert
        result.Should().NotBeNull();
        result!.Title.Should().Be("First Blog Post");
    }

    [Fact]
    public async Task GetFirstOrDefaultAsync_WithNoMatchingPredicate_ShouldReturnNull()
    {
        // Arrange
        using var context = new QueryDbContext(_options);
        var repository = new QueryRepository<BlogPost>(context);
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
        var result = await repository.GetFirstOrDefaultAsync(x => x.CategoryId == 999);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task CountAsync_WithEntities_ShouldReturnCorrectCount()
    {
        // Arrange
        using var context = new QueryDbContext(_options);
        var repository = new QueryRepository<BlogPost>(context);
        var blogPosts = new List<BlogPost>
        {
            new()
            {
                Title = "First Blog Post",
                Content = "First content",
                Slug = "first-blog-post",
                CategoryId = 1,
                AuthorId = "user-1"
            },
            new()
            {
                Title = "Second Blog Post",
                Content = "Second content",
                Slug = "second-blog-post",
                CategoryId = 1,
                AuthorId = "user-2"
            }
        };

        context.Set<BlogPost>().AddRange(blogPosts);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.CountAsync();

        // Assert
        result.Should().Be(2);
    }

    [Fact]
    public async Task CountAsync_WithNoEntities_ShouldReturnZero()
    {
        // Arrange
        using var context = new QueryDbContext(_options);
        var repository = new QueryRepository<BlogPost>(context);

        // Act
        var result = await repository.CountAsync();

        // Assert
        result.Should().Be(0);
    }
} 