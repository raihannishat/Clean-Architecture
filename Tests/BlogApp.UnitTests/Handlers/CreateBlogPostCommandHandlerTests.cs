namespace BlogApp.UnitTests.Handlers;

public class CreateBlogPostCommandHandlerTests
{
    private readonly Mock<IUnitOfWorkFactory> _mockUnitOfWorkFactory;
    private readonly Mock<ICommandUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IOutboxService> _mockOutboxService;
    private readonly Mock<IAutoMapper> _mockMapper;
    private readonly CreateBlogPostCommandHandler _handler;

    public CreateBlogPostCommandHandlerTests()
    {
        _mockUnitOfWorkFactory = new Mock<IUnitOfWorkFactory>();
        _mockUnitOfWork = new Mock<ICommandUnitOfWork>();
        _mockOutboxService = new Mock<IOutboxService>();
        _mockMapper = new Mock<IAutoMapper>();
        _mockUnitOfWorkFactory.Setup(x => x.CreateCommandUnitOfWork()).Returns(_mockUnitOfWork.Object);
        _handler = new CreateBlogPostCommandHandler(_mockUnitOfWorkFactory.Object, _mockOutboxService.Object, _mockMapper.Object);
    }

    [Fact]
    public async Task HandleAsync_WithValidData_ShouldReturnSuccessResponse()
    {
        // Arrange
        var command = new BlogApp.API.Application.Features.Blog.Commands.CreateBlogPostCommand(
            "Test Blog Post",
            "This is a test blog post content.",
            "test-blog-post",
            1,
            "user-id",
            new List<int> { 1, 2 }
        );

        var category = new BlogApp.API.Core.Entities.Category
        {
            Id = 1,
            Name = "Technology",
            Slug = "technology"
        };

        var tags = new List<BlogApp.API.Core.Entities.Tag>
        {
            new BlogApp.API.Core.Entities.Tag { Id = 1, Name = "C#", Slug = "csharp" },
            new BlogApp.API.Core.Entities.Tag { Id = 2, Name = "ASP.NET", Slug = "aspnet" }
        };

        var mockCategoryRepo = new Mock<BlogApp.API.Infrastructure.Persistence.Repositories.Interfaces.ICommandRepository<BlogApp.API.Core.Entities.Category>>();
        var mockTagRepo = new Mock<BlogApp.API.Infrastructure.Persistence.Repositories.Interfaces.ICommandRepository<BlogApp.API.Core.Entities.Tag>>();
        var mockBlogPostRepo = new Mock<BlogApp.API.Infrastructure.Persistence.Repositories.Interfaces.ICommandRepository<BlogApp.API.Core.Entities.BlogPost>>();
        var mockBlogPostTagRepo = new Mock<BlogApp.API.Infrastructure.Persistence.Repositories.Interfaces.ICommandRepository<BlogApp.API.Core.Entities.BlogPostTag>>();

        mockCategoryRepo.Setup(x => x.GetByIdAsync(command.CategoryId))
            .ReturnsAsync(category);

        mockTagRepo.Setup(x => x.GetByIdAsync(It.IsAny<int>()))
            .ReturnsAsync((int id) => tags.FirstOrDefault(t => t.Id == id));

        mockBlogPostRepo.Setup(x => x.AddAsync(It.IsAny<BlogApp.API.Core.Entities.BlogPost>()))
            .Returns(Task.CompletedTask);

        mockBlogPostTagRepo.Setup(x => x.AddAsync(It.IsAny<BlogApp.API.Core.Entities.BlogPostTag>()))
            .Returns(Task.CompletedTask);

        _mockUnitOfWork.Setup(x => x.Repository<BlogApp.API.Core.Entities.Category>()).Returns(mockCategoryRepo.Object);
        _mockUnitOfWork.Setup(x => x.Repository<BlogApp.API.Core.Entities.Tag>()).Returns(mockTagRepo.Object);
        _mockUnitOfWork.Setup(x => x.Repository<BlogApp.API.Core.Entities.BlogPost>()).Returns(mockBlogPostRepo.Object);
        _mockUnitOfWork.Setup(x => x.Repository<BlogApp.API.Core.Entities.BlogPostTag>()).Returns(mockBlogPostTagRepo.Object);
        _mockUnitOfWork.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);

        var expectedDto = new BlogApp.API.Application.Features.Blog.DTOs.BlogPostDTO(1, command.Title, command.Content, command.Slug, "Admin", "Technology", new List<string> { "C#", "ASP.NET" });
        _mockMapper.Setup(x => x.Map<BlogApp.API.Application.Features.Blog.DTOs.BlogPostDTO>(It.IsAny<BlogApp.API.Core.Entities.BlogPost>())).Returns(expectedDto);

        // Act
        var result = await _handler.HandleAsync(command);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Title.Should().Be(command.Title);
        result.Data.Content.Should().Be(command.Content);
    }

    [Fact]
    public async Task HandleAsync_WithNonExistentCategory_ShouldReturnNotFoundResponse()
    {
        // Arrange
        var command = new CreateBlogPostCommand(
            "Test Blog Post",
            "This is a test blog post content.",
            "test-blog-post",
            999,
            "user-id",
            new List<int> { 1, 2 }
        );

        var mockCategoryRepo = new Mock<ICommandRepository<BlogApp.API.Core.Entities.Category>>();
        mockCategoryRepo.Setup(x => x.GetByIdAsync(command.CategoryId))
            .ReturnsAsync((BlogApp.API.Core.Entities.Category?)null);

        _mockUnitOfWork.Setup(x => x.Repository<BlogApp.API.Core.Entities.Category>()).Returns(mockCategoryRepo.Object);

        // Act
        var result = await _handler.HandleAsync(command);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(404);
        result.Message.Should().Contain("Category with ID 999 not found");
    }

    [Fact]
    public async Task HandleAsync_WithNonExistentTag_ShouldReturnNotFoundResponse()
    {
        // Arrange
        var command = new CreateBlogPostCommand(
            "Test Blog Post",
            "This is a test blog post content.",
            "test-blog-post",
            1,
            "user-id",
            new List<int> { 1, 999 }
        );

        var category = new BlogApp.API.Core.Entities.Category
        {
            Id = 1,
            Name = "Technology",
            Slug = "technology"
        };

        var mockCategoryRepo = new Mock<ICommandRepository<BlogApp.API.Core.Entities.Category>>();
        var mockTagRepo = new Mock<ICommandRepository<BlogApp.API.Core.Entities.Tag>>();

        mockCategoryRepo.Setup(x => x.GetByIdAsync(command.CategoryId))
            .ReturnsAsync(category);

        mockTagRepo.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(new BlogApp.API.Core.Entities.Tag { Id = 1, Name = "C#", Slug = "csharp" });

        mockTagRepo.Setup(x => x.GetByIdAsync(999))
            .ReturnsAsync((BlogApp.API.Core.Entities.Tag?)null);

        _mockUnitOfWork.Setup(x => x.Repository<BlogApp.API.Core.Entities.Category>()).Returns(mockCategoryRepo.Object);
        _mockUnitOfWork.Setup(x => x.Repository<BlogApp.API.Core.Entities.Tag>()).Returns(mockTagRepo.Object);

        // Act
        var result = await _handler.HandleAsync(command);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(404);
        result.Message.Should().Contain("Tag with ID 999 not found");
    }
} 