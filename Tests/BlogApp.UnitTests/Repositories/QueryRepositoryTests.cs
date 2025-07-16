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
        var repository = new BlogApp.API.Infrastructure.Persistence.Repositories.Implementations.QueryRepository<BlogApp.API.Core.Entities.BlogPost>(context);
        var blogPost = new BlogApp.API.Core.Entities.BlogPost
        {
            Title = "Test Blog Post",
            Content = "Test content",
            Slug = "test-blog-post",
            CategoryId = 1,
            AuthorId = "user-id"
        };

        context.Set<BlogApp.API.Core.Entities.BlogPost>().Add(blogPost);
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
        var repository = new BlogApp.API.Infrastructure.Persistence.Repositories.Implementations.QueryRepository<BlogApp.API.Core.Entities.BlogPost>(context);

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
        var repository = new BlogApp.API.Infrastructure.Persistence.Repositories.Implementations.QueryRepository<BlogApp.API.Core.Entities.BlogPost>(context);
        var blogPosts = new List<BlogApp.API.Core.Entities.BlogPost>
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

        context.Set<BlogApp.API.Core.Entities.BlogPost>().AddRange(blogPosts);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetAllAsync();

        // Assert
        result.Should().NotBeNull();
        result.Count().Should().Be(2);
        result.Should().Contain(x => x.Title == "First Blog Post");
        result.Should().Contain(x => x.Title == "Second Blog Post");
    }

    [Fact]
    public async Task GetAllAsync_WithNoEntities_ShouldReturnEmptyList()
    {
        // Arrange
        using var context = new QueryDbContext(_options);
        var repository = new BlogApp.API.Infrastructure.Persistence.Repositories.Implementations.QueryRepository<BlogApp.API.Core.Entities.BlogPost>(context);

        // Act
        var result = await repository.GetAllAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    // Remove usages of FindAsync, CountAsync, GetFirstOrDefaultAsync, etc. Use supported repository methods or mock data instead.
    // Fix Enumerable.Count usage to be a method call.
} 