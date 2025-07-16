namespace BlogApp.UnitTests.Handlers;

public class SearchPostsQueryHandlerTests
{
    private readonly Mock<IQueryUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IUnitOfWorkFactory> _mockUnitOfWorkFactory;
    private readonly Mock<IMapper> _mockMapper;
    private readonly SearchPostsQueryHandler _handler;

    public SearchPostsQueryHandlerTests()
    {
        _mockUnitOfWork = new Mock<IQueryUnitOfWork>();
        _mockUnitOfWorkFactory = new Mock<IUnitOfWorkFactory>();
        _mockMapper = new Mock<IMapper>();
        _mockUnitOfWorkFactory.Setup(x => x.CreateQueryUnitOfWork()).Returns(_mockUnitOfWork.Object);
        _handler = new SearchPostsQueryHandler(_mockUnitOfWorkFactory.Object, _mockMapper.Object);
    }

    [Fact]
    public async Task HandleAsync_WithMatchingPosts_ShouldReturnFilteredResults()
    {
        // Arrange
        var posts = new List<BlogPost>
        {
            new() { Id = 1, Title = "ASP.NET Core", Content = "Learn ASP.NET Core", IsPublished = true },
            new() { Id = 2, Title = "EF Core", Content = "Entity Framework Core Guide", IsPublished = true },
            new() { Id = 3, Title = "Angular", Content = "Angular for Beginners", IsPublished = false }
        };
        var mockBlogPostRepo = new Mock<IQueryRepository<BlogPost>>();
        mockBlogPostRepo.Setup(x => x.GetAllAsync()).ReturnsAsync(posts);
        _mockUnitOfWork.Setup(x => x.Repository<BlogPost>()).Returns(mockBlogPostRepo.Object);
        _mockMapper.Setup(m => m.Map<BlogPostDTO>(It.IsAny<BlogPost>())).Returns((BlogPost p) => new BlogPostDTO { Id = p.Id, Title = p.Title });
        var query = new SearchPostsQuery("Core", 1, 10, false);
        // Act
        var result = await _handler.HandleAsync(query);
        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Count.Should().Be(1);
        result.Data[0].Title.Should().Be("ASP.NET Core");
    }

    [Fact]
    public async Task HandleAsync_WithIncludeUnpublished_ShouldReturnAllMatching()
    {
        // Arrange
        var posts = new List<BlogPost>
        {
            new() { Id = 1, Title = "Angular", Content = "Angular for Beginners", IsPublished = false },
            new() { Id = 2, Title = "Angular Advanced", Content = "Advanced Angular", IsPublished = true }
        };
        var mockBlogPostRepo = new Mock<IQueryRepository<BlogPost>>();
        mockBlogPostRepo.Setup(x => x.GetAllAsync()).ReturnsAsync(posts);
        _mockUnitOfWork.Setup(x => x.Repository<BlogPost>()).Returns(mockBlogPostRepo.Object);
        _mockMapper.Setup(m => m.Map<BlogPostDTO>(It.IsAny<BlogPost>())).Returns((BlogPost p) => new BlogPostDTO { Id = p.Id, Title = p.Title });
        var query = new SearchPostsQuery("Angular", 1, 10, true);
        // Act
        var result = await _handler.HandleAsync(query);
        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Count.Should().Be(2);
    }

    [Fact]
    public async Task HandleAsync_WithPaging_ShouldReturnPagedResults()
    {
        // Arrange
        var posts = Enumerable.Range(1, 25).Select(i => new BlogPost
        {
            Id = i,
            Title = $"Post {i}",
            Content = $"Content {i}",
            IsPublished = true
        }).ToList();
        var mockBlogPostRepo = new Mock<IQueryRepository<BlogPost>>();
        mockBlogPostRepo.Setup(x => x.GetAllAsync()).ReturnsAsync(posts);
        _mockUnitOfWork.Setup(x => x.Repository<BlogPost>()).Returns(mockBlogPostRepo.Object);
        _mockMapper.Setup(m => m.Map<BlogPostDTO>(It.IsAny<BlogPost>())).Returns((BlogPost p) => new BlogPostDTO { Id = p.Id, Title = p.Title });
        var query = new SearchPostsQuery("Post", 2, 10, false);
        // Act
        var result = await _handler.HandleAsync(query);
        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Count.Should().Be(10);
        result.Data[0].Title.Should().Be("Post 11");
    }
} 