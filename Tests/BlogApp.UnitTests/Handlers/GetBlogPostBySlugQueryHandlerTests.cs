namespace BlogApp.UnitTests.Handlers;

public class GetBlogPostBySlugQueryHandlerTests
{
    private readonly Mock<IUnitOfWorkFactory> _mockUnitOfWorkFactory;
    private readonly Mock<IQueryUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IAutoMapper> _mockMapper;
    private readonly GetBlogPostBySlugQueryHandler _handler;

    public GetBlogPostBySlugQueryHandlerTests()
    {
        _mockUnitOfWorkFactory = new Mock<IUnitOfWorkFactory>();
        _mockUnitOfWork = new Mock<IQueryUnitOfWork>();
        _mockMapper = new Mock<IAutoMapper>();
        _mockUnitOfWorkFactory.Setup(x => x.CreateQueryUnitOfWork()).Returns(_mockUnitOfWork.Object);
        _handler = new GetBlogPostBySlugQueryHandler(_mockUnitOfWorkFactory.Object, _mockMapper.Object);
    }

    [Fact]
    public async Task HandleAsync_WithValidSlug_ShouldReturnSuccessResponse()
    {
        // Arrange
        var query = new BlogApp.API.Application.Features.Blog.Queries.GetBlogPostBySlugQuery("test-blog-post");

        var blogPost = new BlogApp.API.Core.Entities.BlogPost
        {
            Id = 1,
            Title = "Test Blog Post",
            Content = "This is a test blog post content.",
            Slug = "test-blog-post",
            CategoryId = 1,
            AuthorId = "user-id",
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            UpdatedAt = DateTime.UtcNow
        };

        var mockBlogPostRepo = new Mock<IQueryRepository<BlogApp.API.Core.Entities.BlogPost>>();
        mockBlogPostRepo.Setup(x => x.GetAllAsync())
            .ReturnsAsync(new List<BlogApp.API.Core.Entities.BlogPost> { blogPost });

        _mockUnitOfWork.Setup(x => x.Repository<BlogApp.API.Core.Entities.BlogPost>()).Returns(mockBlogPostRepo.Object);
        _mockMapper.Setup(x => x.Map<BlogPostDTO>(It.IsAny<BlogApp.API.Core.Entities.BlogPost>())).Returns(new BlogPostDTO(1, blogPost.Title, blogPost.Content, blogPost.Slug, "Admin", "Technology", new List<string>()));

        // Act
        var result = await _handler.HandleAsync(query);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Title.Should().Be("Test Blog Post");
        result.Data.Slug.Should().Be("test-blog-post");
        result.Data.Content.Should().Be("This is a test blog post content.");
    }

    [Fact]
    public async Task HandleAsync_WithNonExistentSlug_ShouldReturnNotFoundResponse()
    {
        // Arrange
        var query = new BlogApp.API.Application.Features.Blog.Queries.GetBlogPostBySlugQuery("non-existent-slug");

        var mockBlogPostRepo = new Mock<IQueryRepository<BlogApp.API.Core.Entities.BlogPost>>();
        mockBlogPostRepo.Setup(x => x.GetAllAsync())
            .ReturnsAsync(new List<BlogApp.API.Core.Entities.BlogPost>());

        _mockUnitOfWork.Setup(x => x.Repository<BlogApp.API.Core.Entities.BlogPost>()).Returns(mockBlogPostRepo.Object);

        // Act
        var result = await _handler.HandleAsync(query);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(404);
        result.Message.Should().Contain("not found");
    }

    [Fact]
    public async Task HandleAsync_WithEmptySlug_ShouldReturnValidationError()
    {
        // Arrange
        var query = new BlogApp.API.Application.Features.Blog.Queries.GetBlogPostBySlugQuery("");

        // Act
        var result = await _handler.HandleAsync(query);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(400);
        result.Errors.Should().Contain("Slug is required");
    }

    [Fact]
    public async Task HandleAsync_WithNullSlug_ShouldReturnValidationError()
    {
        // Arrange
        var query = new BlogApp.API.Application.Features.Blog.Queries.GetBlogPostBySlugQuery(null!);

        // Act
        var result = await _handler.HandleAsync(query);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(400);
        result.Errors.Should().Contain("Slug is required");
    }
} 