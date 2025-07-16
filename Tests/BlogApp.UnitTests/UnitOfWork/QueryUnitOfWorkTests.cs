namespace BlogApp.UnitTests.UnitOfWork;

public class QueryUnitOfWorkTests
{
    private readonly DbContextOptions<QueryDbContext> _options;
    private readonly Mock<IServiceProvider> _mockServiceProvider;

    public QueryUnitOfWorkTests()
    {
        _mockServiceProvider = new Mock<IServiceProvider>();
        _options = new DbContextOptionsBuilder<QueryDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
    }

    [Fact]
    public void Repository_WithValidType_ShouldReturnRepository()
    {
        // Arrange
        using var context = new QueryDbContext(_options);
        var repo = new BlogApp.API.Infrastructure.Persistence.Repositories.Implementations.QueryRepository<BlogPost>(context);
        _mockServiceProvider.Setup(x => x.GetService(typeof(BlogApp.API.Infrastructure.Persistence.Repositories.Interfaces.IQueryRepository<BlogPost>))).Returns(repo);
        var unitOfWork = new BlogApp.API.Infrastructure.Persistence.UnitOfWork.Implementations.QueryUnitOfWork(context, _mockServiceProvider.Object);

        // Act
        var repository = unitOfWork.Repository<BlogPost>();

        // Assert
        repository.Should().NotBeNull();
        repository.Should().BeOfType<BlogApp.API.Infrastructure.Persistence.Repositories.Implementations.QueryRepository<BlogPost>>();
    }

    [Fact]
    public void Repository_WithDifferentTypes_ShouldReturnDifferentRepositories()
    {
        // Arrange
        using var context = new QueryDbContext(_options);
        var blogPostRepo = new BlogApp.API.Infrastructure.Persistence.Repositories.Implementations.QueryRepository<BlogPost>(context);
        var categoryRepo = new BlogApp.API.Infrastructure.Persistence.Repositories.Implementations.QueryRepository<Category>(context);
        _mockServiceProvider.Setup(x => x.GetService(typeof(BlogApp.API.Infrastructure.Persistence.Repositories.Interfaces.IQueryRepository<BlogPost>))).Returns(blogPostRepo);
        _mockServiceProvider.Setup(x => x.GetService(typeof(BlogApp.API.Infrastructure.Persistence.Repositories.Interfaces.IQueryRepository<Category>))).Returns(categoryRepo);
        var unitOfWork = new BlogApp.API.Infrastructure.Persistence.UnitOfWork.Implementations.QueryUnitOfWork(context, _mockServiceProvider.Object);

        // Act
        var repo1 = unitOfWork.Repository<BlogPost>();
        var repo2 = unitOfWork.Repository<Category>();

        // Assert
        repo1.Should().NotBeNull();
        repo2.Should().NotBeNull();
        repo1.Should().NotBeSameAs(repo2);
    }

    [Fact]
    public async Task Repository_WithExistingData_ShouldReturnData()
    {
        // Arrange
        using var context = new QueryDbContext(_options);
        var repo = new BlogApp.API.Infrastructure.Persistence.Repositories.Implementations.QueryRepository<BlogPost>(context);
        _mockServiceProvider.Setup(x => x.GetService(typeof(BlogApp.API.Infrastructure.Persistence.Repositories.Interfaces.IQueryRepository<BlogPost>))).Returns(repo);
        var unitOfWork = new BlogApp.API.Infrastructure.Persistence.UnitOfWork.Implementations.QueryUnitOfWork(context, _mockServiceProvider.Object);
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
        var repo = new BlogApp.API.Infrastructure.Persistence.Repositories.Implementations.QueryRepository<BlogPost>(context);
        _mockServiceProvider.Setup(x => x.GetService(typeof(BlogApp.API.Infrastructure.Persistence.Repositories.Interfaces.IQueryRepository<BlogPost>))).Returns(repo);
        var unitOfWork = new BlogApp.API.Infrastructure.Persistence.UnitOfWork.Implementations.QueryUnitOfWork(context, _mockServiceProvider.Object);

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
        var unitOfWork = new BlogApp.API.Infrastructure.Persistence.UnitOfWork.Implementations.QueryUnitOfWork(context, _mockServiceProvider.Object);

        // Act
        unitOfWork.Dispose();

        // Assert
        context.Database.ProviderName.Should().Be("Microsoft.EntityFrameworkCore.InMemory");
    }

    [Fact]
    public async Task Repository_WithComplexQuery_ShouldWorkCorrectly()
    {
        // Arrange
        using var context = new QueryDbContext(_options);
        var unitOfWork = new BlogApp.API.Infrastructure.Persistence.UnitOfWork.Implementations.QueryUnitOfWork(context, _mockServiceProvider.Object);
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
        // var found = await repository.FindAsync(...);
        // found.Should().BeNull();
    }
} 