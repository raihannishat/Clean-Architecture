namespace BlogApp.UnitTests.Handlers;

public class GetBlogPostsQueryHandlerTests
{
    private readonly Mock<IUnitOfWorkFactory> _mockUnitOfWorkFactory;
    private readonly Mock<IQueryUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IAutoMapper> _mockMapper;
    private readonly GetBlogPostsQueryHandler _handler;

    public GetBlogPostsQueryHandlerTests()
    {
        _mockUnitOfWorkFactory = new Mock<IUnitOfWorkFactory>();
        _mockUnitOfWork = new Mock<IQueryUnitOfWork>();
        _mockMapper = new Mock<IAutoMapper>();
        _mockUnitOfWorkFactory.Setup(x => x.CreateQueryUnitOfWork()).Returns(_mockUnitOfWork.Object);
        _handler = new BlogApp.API.Application.Features.Blog.Queries.GetBlogPostsQueryHandler(_mockUnitOfWorkFactory.Object, _mockMapper.Object);
    }

    [Fact]
    public async Task HandleAsync_WithValidQuery_ShouldReturnSuccessResponse()
    {
        // Arrange
        var query = new BlogApp.API.Application.Features.Blog.Queries.GetBlogPostsQuery(1, 10);

        var blogPosts = new List<BlogApp.API.Core.Entities.BlogPost>
        {
            new BlogApp.API.Core.Entities.BlogPost
            {
                Id = 1,
                Title = "First Blog Post",
                Content = "First content",
                Slug = "first-blog-post",
                CategoryId = 1,
                AuthorId = "user-1",
                CreatedAt = DateTime.UtcNow.AddDays(-1)
            },
            new BlogApp.API.Core.Entities.BlogPost
            {
                Id = 2,
                Title = "Second Blog Post",
                Content = "Second content",
                Slug = "second-blog-post",
                CategoryId = 1,
                AuthorId = "user-2",
                CreatedAt = DateTime.UtcNow
            }
        };

        var mockBlogPostRepo = new Mock<BlogApp.API.Infrastructure.Persistence.Repositories.Interfaces.IQueryRepository<BlogApp.API.Core.Entities.BlogPost>>();
        mockBlogPostRepo.Setup(x => x.GetAllAsync())
            .ReturnsAsync(blogPosts);

        _mockUnitOfWork.Setup(x => x.Repository<BlogApp.API.Core.Entities.BlogPost>()).Returns(mockBlogPostRepo.Object);
        _mockMapper.Setup(x => x.Map<BlogApp.API.Application.Features.Blog.DTOs.BlogPostDTO>(It.IsAny<BlogApp.API.Core.Entities.BlogPost>()))
            .Returns((BlogApp.API.Core.Entities.BlogPost p) => new BlogApp.API.Application.Features.Blog.DTOs.BlogPostDTO(p.Id, p.Title, p.Content, p.Slug, p.AuthorId, "Category", new List<string>()));

        // Act
        var result = await _handler.HandleAsync(query);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Count.Should().Be(2);
        result.Data[0].Title.Should().Be("First Blog Post");
        result.Data[1].Title.Should().Be("Second Blog Post");
    }

    [Fact]
    public async Task HandleAsync_WithPagination_ShouldReturnPaginatedResults()
    {
        // Arrange
        var query = new BlogApp.API.Application.Features.Blog.Queries.GetBlogPostsQuery(2, 5);

        var allBlogPosts = Enumerable.Range(1, 15)
            .Select(i => new BlogApp.API.Core.Entities.BlogPost
            {
                Id = i,
                Title = $"Blog Post {i}",
                Content = $"Content {i}",
                Slug = $"blog-post-{i}",
                CategoryId = 1,
                AuthorId = $"user-{i}",
                CreatedAt = DateTime.UtcNow.AddDays(-i)
            })
            .ToList();

        var mockBlogPostRepo = new Mock<BlogApp.API.Infrastructure.Persistence.Repositories.Interfaces.IQueryRepository<BlogApp.API.Core.Entities.BlogPost>>();
        mockBlogPostRepo.Setup(x => x.GetAllAsync())
            .ReturnsAsync(allBlogPosts);

        _mockUnitOfWork.Setup(x => x.Repository<BlogApp.API.Core.Entities.BlogPost>()).Returns(mockBlogPostRepo.Object);
        _mockMapper.Setup(x => x.Map<BlogApp.API.Application.Features.Blog.DTOs.BlogPostDTO>(It.IsAny<BlogApp.API.Core.Entities.BlogPost>()))
            .Returns((BlogApp.API.Core.Entities.BlogPost p) => new BlogApp.API.Application.Features.Blog.DTOs.BlogPostDTO(p.Id, p.Title, p.Content, p.Slug, p.AuthorId, "Category", new List<string>()));

        // Act
        var result = await _handler.HandleAsync(query);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Count.Should().Be(5);
    }

    [Fact]
    public async Task HandleAsync_WithNoBlogPosts_ShouldReturnEmptyList()
    {
        // Arrange
        var query = new BlogApp.API.Application.Features.Blog.Queries.GetBlogPostsQuery(1, 10);

        var mockBlogPostRepo = new Mock<BlogApp.API.Infrastructure.Persistence.Repositories.Interfaces.IQueryRepository<BlogApp.API.Core.Entities.BlogPost>>();
        mockBlogPostRepo.Setup(x => x.GetAllAsync())
            .ReturnsAsync(new List<BlogApp.API.Core.Entities.BlogPost>());

        _mockUnitOfWork.Setup(x => x.Repository<BlogApp.API.Core.Entities.BlogPost>()).Returns(mockBlogPostRepo.Object);
        _mockMapper.Setup(x => x.Map<BlogApp.API.Application.Features.Blog.DTOs.BlogPostDTO>(It.IsAny<BlogApp.API.Core.Entities.BlogPost>()))
            .Returns((BlogApp.API.Core.Entities.BlogPost p) => new BlogApp.API.Application.Features.Blog.DTOs.BlogPostDTO(p.Id, p.Title, p.Content, p.Slug, p.AuthorId, "Category", new List<string>()));

        // Act
        var result = await _handler.HandleAsync(query);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Count.Should().Be(0);
    }

    [Fact]
    public async Task HandleAsync_WithInvalidPage_ShouldReturnValidationError()
    {
        // Arrange
        var query = new BlogApp.API.Application.Features.Blog.Queries.GetBlogPostsQuery(0, 10);

        // Act
        var result = await _handler.HandleAsync(query);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(400);
        result.Errors.Should().Contain("Page must be greater than 0");
    }

    [Fact]
    public async Task HandleAsync_WithInvalidPageSize_ShouldReturnValidationError()
    {
        // Arrange
        var query = new BlogApp.API.Application.Features.Blog.Queries.GetBlogPostsQuery(1, 0);

        // Act
        var result = await _handler.HandleAsync(query);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(400);
        result.Errors.Should().Contain("Page size must be greater than 0");
    }
} 