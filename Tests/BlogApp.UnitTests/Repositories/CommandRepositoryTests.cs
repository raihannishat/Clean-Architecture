using BlogApp.API.Core.Entities;
using BlogApp.API.Infrastructure.Persistence.Repositories.Implementations;
using BlogApp.API.Infrastructure.Persistence.Contexts;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace BlogApp.UnitTests.Repositories;

public class CommandRepositoryTests
{
    private readonly DbContextOptions<CommandDbContext> _options;

    public CommandRepositoryTests()
    {
        _options = new DbContextOptionsBuilder<CommandDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
    }

    [Fact]
    public async Task AddAsync_WithValidEntity_ShouldAddToDatabase()
    {
        // Arrange
        using var context = new CommandDbContext(_options);
        var repository = new CommandRepository<BlogPost>(context);
        var blogPost = new BlogPost
        {
            Title = "Test Blog Post",
            Content = "Test content",
            Slug = "test-blog-post",
            CategoryId = 1,
            AuthorId = "user-id"
        };

        // Act
        await repository.AddAsync(blogPost);
        await context.SaveChangesAsync();

        // Assert
        var savedBlogPost = await context.Set<BlogPost>().FirstOrDefaultAsync(x => x.Slug == "test-blog-post");
        savedBlogPost.Should().NotBeNull();
        savedBlogPost!.Title.Should().Be("Test Blog Post");
    }

    [Fact]
    public async Task UpdateAsync_WithValidEntity_ShouldUpdateInDatabase()
    {
        // Arrange
        using var context = new CommandDbContext(_options);
        var repository = new CommandRepository<BlogPost>(context);
        var blogPost = new BlogPost
        {
            Title = "Original Title",
            Content = "Original content",
            Slug = "test-blog-post",
            CategoryId = 1,
            AuthorId = "user-id"
        };

        await repository.AddAsync(blogPost);
        await context.SaveChangesAsync();

        // Act
        blogPost.Title = "Updated Title";
        repository.Update(blogPost);
        await context.SaveChangesAsync();

        // Assert
        var updatedBlogPost = await context.Set<BlogPost>().FirstOrDefaultAsync(x => x.Slug == "test-blog-post");
        updatedBlogPost.Should().NotBeNull();
        updatedBlogPost!.Title.Should().Be("Updated Title");
    }

    [Fact]
    public async Task DeleteAsync_WithValidEntity_ShouldRemoveFromDatabase()
    {
        // Arrange
        using var context = new CommandDbContext(_options);
        var repository = new CommandRepository<BlogPost>(context);
        var blogPost = new BlogPost
        {
            Title = "Test Blog Post",
            Content = "Test content",
            Slug = "test-blog-post",
            CategoryId = 1,
            AuthorId = "user-id"
        };

        await repository.AddAsync(blogPost);
        await context.SaveChangesAsync();

        // Act
        repository.Delete(blogPost);
        await context.SaveChangesAsync();

        // Assert
        var deletedBlogPost = await context.Set<BlogPost>().FirstOrDefaultAsync(x => x.Slug == "test-blog-post");
        deletedBlogPost.Should().BeNull();
    }

    [Fact]
    public async Task GetByIdAsync_WithExistingId_ShouldReturnEntity()
    {
        // Arrange
        using var context = new CommandDbContext(_options);
        var repository = new CommandRepository<BlogPost>(context);
        var blogPost = new BlogPost
        {
            Title = "Test Blog Post",
            Content = "Test content",
            Slug = "test-blog-post",
            CategoryId = 1,
            AuthorId = "user-id"
        };

        await repository.AddAsync(blogPost);
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
        using var context = new CommandDbContext(_options);
        var repository = new CommandRepository<BlogPost>(context);

        // Act
        var result = await repository.GetByIdAsync(999);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAllAsync_WithEntities_ShouldReturnAllEntities()
    {
        // Arrange
        using var context = new CommandDbContext(_options);
        var repository = new CommandRepository<BlogPost>(context);
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

        foreach (var post in blogPosts)
        {
            await repository.AddAsync(post);
        }
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
        using var context = new CommandDbContext(_options);
        var repository = new CommandRepository<BlogPost>(context);

        // Act
        var result = await repository.GetAllAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }
} 