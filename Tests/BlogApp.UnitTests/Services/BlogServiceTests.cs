namespace BlogApp.UnitTests.Services;

public class BlogServiceTests
{
    private readonly Mock<IQueryUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IQueryRepository<BlogApp.API.Core.Entities.BlogPost>> _mockBlogPostRepo;
    private readonly Mock<IQueryRepository<BlogApp.API.Core.Entities.Category>> _mockCategoryRepo;
    private readonly Mock<IQueryRepository<BlogApp.API.Core.Entities.Tag>> _mockTagRepo;
    private readonly BlogService _blogService;

    public BlogServiceTests()
    {
        _mockUnitOfWork = new Mock<IQueryUnitOfWork>();
        _mockBlogPostRepo = new Mock<IQueryRepository<BlogApp.API.Core.Entities.BlogPost>>();
        _mockCategoryRepo = new Mock<IQueryRepository<BlogApp.API.Core.Entities.Category>>();
        _mockTagRepo = new Mock<IQueryRepository<BlogApp.API.Core.Entities.Tag>>();
        _blogService = new BlogService(_mockUnitOfWork.Object);
    }

    [Fact]
    public async Task GetBlogPostsAsync_WithValidParameters_ShouldReturnSuccessResponse()
    {
        // Arrange
        var page = 1;
        var pageSize = 10;
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
        _mockBlogPostRepo.Setup(x => x.GetAllAsync()).ReturnsAsync(blogPosts);
        _mockUnitOfWork.Setup(x => x.Repository<BlogApp.API.Core.Entities.BlogPost>()).Returns(_mockBlogPostRepo.Object);

        // Act
        var result = await _blogService.GetBlogPostsAsync(page, pageSize);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
    }

    [Fact]
    public async Task GetBlogPostBySlugAsync_WithValidSlug_ShouldReturnSuccessResponse()
    {
        // Arrange
        var slug = "test-blog-post";
        var blogPost = new BlogApp.API.Core.Entities.BlogPost
        {
            Id = 1,
            Title = "Test Blog Post",
            Content = "Test content",
            Slug = "test-blog-post",
            CategoryId = 1,
            AuthorId = "user-id",
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            UpdatedAt = DateTime.UtcNow
        };
        _mockBlogPostRepo.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<BlogApp.API.Core.Entities.BlogPost> { blogPost });
        _mockUnitOfWork.Setup(x => x.Repository<BlogApp.API.Core.Entities.BlogPost>()).Returns(_mockBlogPostRepo.Object);

        // Act
        var result = await _blogService.GetBlogPostBySlugAsync(slug);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
    }

    [Fact]
    public async Task GetBlogPostBySlugAsync_WithNonExistentSlug_ShouldReturnNotFoundResponse()
    {
        // Arrange
        var slug = "non-existent-slug";
        _mockBlogPostRepo.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<BlogApp.API.Core.Entities.BlogPost>());
        _mockUnitOfWork.Setup(x => x.Repository<BlogApp.API.Core.Entities.BlogPost>()).Returns(_mockBlogPostRepo.Object);

        // Act
        var result = await _blogService.GetBlogPostBySlugAsync(slug);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(404);
        result.Message.Should().Contain("Blog post not found");
    }

    [Fact]
    public async Task GetCategoriesAsync_ShouldReturnSuccessResponse()
    {
        // Arrange
        var categories = new List<BlogApp.API.Core.Entities.Category>
        {
            new()
            {
                Id = 1,
                Name = "Technology",
                Slug = "technology",
                Color = "#007bff"
            },
            new()
            {
                Id = 2,
                Name = "Travel",
                Slug = "travel",
                Color = "#28a745"
            }
        };
        _mockCategoryRepo.Setup(x => x.GetAllAsync()).ReturnsAsync(categories);
        _mockUnitOfWork.Setup(x => x.Repository<BlogApp.API.Core.Entities.Category>()).Returns(_mockCategoryRepo.Object);

        // Act
        var result = await _blogService.GetCategoriesAsync();

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
    }

    [Fact]
    public async Task GetTagsAsync_ShouldReturnSuccessResponse()
    {
        // Arrange
        var tags = new List<BlogApp.API.Core.Entities.Tag>
        {
            new()
            {
                Id = 1,
                Name = "C#",
                Slug = "csharp",
                Color = "#007bff"
            },
            new()
            {
                Id = 2,
                Name = "ASP.NET",
                Slug = "aspnet",
                Color = "#28a745"
            }
        };
        _mockTagRepo.Setup(x => x.GetAllAsync()).ReturnsAsync(tags);
        _mockUnitOfWork.Setup(x => x.Repository<BlogApp.API.Core.Entities.Tag>()).Returns(_mockTagRepo.Object);

        // Act
        var result = await _blogService.GetTagsAsync();

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
    }

    // [Fact]
    // public async Task CreateBlogPostAsync_WithValidData_ShouldReturnSuccessResponse()
    // {
    //     // Arrange
    //     var blogPost = new BlogApp.API.Core.Entities.BlogPost
    //     {
    //         Title = "New Blog Post",
    //         Content = "This is a new blog post content.",
    //         Slug = "new-blog-post",
    //         CategoryId = 1,
    //         AuthorId = "user-id"
    //     };

    //     // Act
    //     var result = await _blogService.CreateBlogPostAsync(blogPost);

    //     // Assert
    //     result.Should().NotBeNull();
    //     result.IsSuccess.Should().BeTrue();
    //     result.Data.Should().NotBeNull();
    //     result.Data!.Title.Should().Be("New Blog Post");
    // }
} 