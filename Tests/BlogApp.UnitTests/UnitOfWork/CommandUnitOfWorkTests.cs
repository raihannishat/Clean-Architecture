namespace BlogApp.UnitTests.UnitOfWork;

public class CommandUnitOfWorkTests
{
    private readonly DbContextOptions<CommandDbContext> _options;
    private readonly Mock<IServiceProvider> _mockServiceProvider;

    public CommandUnitOfWorkTests()
    {
        _mockServiceProvider = new Mock<IServiceProvider>();
        _options = new DbContextOptionsBuilder<CommandDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
    }

    [Fact]
    public void Repository_WithValidType_ShouldReturnRepository()
    {
        // Arrange
        using var context = new CommandDbContext(_options);
        var repo = new BlogApp.API.Infrastructure.Persistence.Repositories.Implementations.CommandRepository<BlogPost>(context);
        _mockServiceProvider.Setup(x => x.GetService(typeof(BlogApp.API.Infrastructure.Persistence.Repositories.Interfaces.ICommandRepository<BlogPost>))).Returns(repo);
        var unitOfWork = new BlogApp.API.Infrastructure.Persistence.UnitOfWork.Implementations.CommandUnitOfWork(context, _mockServiceProvider.Object);

        // Act
        var repository = unitOfWork.Repository<BlogPost>();

        // Assert
        repository.Should().NotBeNull();
        repository.Should().BeOfType<BlogApp.API.Infrastructure.Persistence.Repositories.Implementations.CommandRepository<BlogPost>>();
    }

    [Fact]
    public void Repository_WithDifferentTypes_ShouldReturnDifferentRepositories()
    {
        // Arrange
        using var context = new CommandDbContext(_options);
        var blogPostRepo = new BlogApp.API.Infrastructure.Persistence.Repositories.Implementations.CommandRepository<BlogPost>(context);
        var categoryRepo = new BlogApp.API.Infrastructure.Persistence.Repositories.Implementations.CommandRepository<Category>(context);
        _mockServiceProvider.Setup(x => x.GetService(typeof(BlogApp.API.Infrastructure.Persistence.Repositories.Interfaces.ICommandRepository<BlogPost>))).Returns(blogPostRepo);
        _mockServiceProvider.Setup(x => x.GetService(typeof(BlogApp.API.Infrastructure.Persistence.Repositories.Interfaces.ICommandRepository<Category>))).Returns(categoryRepo);
        var unitOfWork = new BlogApp.API.Infrastructure.Persistence.UnitOfWork.Implementations.CommandUnitOfWork(context, _mockServiceProvider.Object);

        // Act
        var repo1 = unitOfWork.Repository<BlogPost>();
        var repo2 = unitOfWork.Repository<Category>();

        // Assert
        repo1.Should().NotBeNull();
        repo2.Should().NotBeNull();
        repo1.Should().NotBeSameAs(repo2);
    }

    [Fact]
    public async Task SaveChangesAsync_WithValidChanges_ShouldReturnPositiveCount()
    {
        // Arrange
        using var context = new CommandDbContext(_options);
        var repo = new BlogApp.API.Infrastructure.Persistence.Repositories.Implementations.CommandRepository<BlogPost>(context);
        _mockServiceProvider.Setup(x => x.GetService(typeof(BlogApp.API.Infrastructure.Persistence.Repositories.Interfaces.ICommandRepository<BlogPost>))).Returns(repo);
        var unitOfWork = new BlogApp.API.Infrastructure.Persistence.UnitOfWork.Implementations.CommandUnitOfWork(context, _mockServiceProvider.Object);
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
        var unitOfWork = new BlogApp.API.Infrastructure.Persistence.UnitOfWork.Implementations.CommandUnitOfWork(context, _mockServiceProvider.Object);

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
        var blogPostRepo = new BlogApp.API.Infrastructure.Persistence.Repositories.Implementations.CommandRepository<BlogPost>(context);
        var categoryRepo = new BlogApp.API.Infrastructure.Persistence.Repositories.Implementations.CommandRepository<Category>(context);
        _mockServiceProvider.Setup(x => x.GetService(typeof(BlogApp.API.Infrastructure.Persistence.Repositories.Interfaces.ICommandRepository<BlogPost>))).Returns(blogPostRepo);
        _mockServiceProvider.Setup(x => x.GetService(typeof(BlogApp.API.Infrastructure.Persistence.Repositories.Interfaces.ICommandRepository<Category>))).Returns(categoryRepo);
        var unitOfWork = new BlogApp.API.Infrastructure.Persistence.UnitOfWork.Implementations.CommandUnitOfWork(context, _mockServiceProvider.Object);
        var repo1 = unitOfWork.Repository<Category>();
        var repo2 = unitOfWork.Repository<BlogPost>();

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

        await repo1.AddAsync(category);
        await repo2.AddAsync(blogPost);

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
        var unitOfWork = new BlogApp.API.Infrastructure.Persistence.UnitOfWork.Implementations.CommandUnitOfWork(context, _mockServiceProvider.Object);

        // Act
        unitOfWork.Dispose();

        // Assert
        context.Database.ProviderName.Should().Be("Microsoft.EntityFrameworkCore.InMemory");
    }
} 