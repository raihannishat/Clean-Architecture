namespace BlogApp.UnitTests.Handlers;

public class CreateCommentCommandHandlerTests
{
    private readonly Mock<IUnitOfWorkFactory> _mockUnitOfWorkFactory;
    private readonly Mock<ICommandUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IOutboxService> _mockOutboxService;
    private readonly Mock<IAutoMapper> _mockMapper;
    private readonly CreateCommentCommandHandler _handler;

    public CreateCommentCommandHandlerTests()
    {
        _mockUnitOfWorkFactory = new Mock<IUnitOfWorkFactory>();
        _mockUnitOfWork = new Mock<ICommandUnitOfWork>();
        _mockOutboxService = new Mock<IOutboxService>();
        _mockMapper = new Mock<IAutoMapper>();
        _mockUnitOfWorkFactory.Setup(x => x.CreateCommandUnitOfWork()).Returns(_mockUnitOfWork.Object);
        _handler = new BlogApp.API.Application.Features.Comment.Commands.CreateCommentCommandHandler(_mockUnitOfWorkFactory.Object, _mockOutboxService.Object, _mockMapper.Object);
    }

    private static BlogApp.API.Application.Features.Comment.Commands.CreateCommentCommand CreateCommand(
        string content = "Test comment",
        int blogPostId = 1,
        string authorId = "user-1",
        int? parentCommentId = null)
    {
        return new BlogApp.API.Application.Features.Comment.Commands.CreateCommentCommand(content, blogPostId, authorId, parentCommentId);
    }

    private void SetupBlogPostRepo(int blogPostId, BlogApp.API.Core.Entities.BlogPost? blogPost)
    {
        var mockBlogPostRepo = new Mock<ICommandRepository<BlogApp.API.Core.Entities.BlogPost>>();
        mockBlogPostRepo.Setup(x => x.GetByIdAsync(blogPostId)).ReturnsAsync(blogPost);
        _mockUnitOfWork.Setup(x => x.Repository<BlogApp.API.Core.Entities.BlogPost>()).Returns(mockBlogPostRepo.Object);
    }

    private void SetupCommentRepo(int? parentCommentId = null, BlogApp.API.Core.Entities.Comment? parentComment = null)
    {
        var mockCommentRepo = new Mock<ICommandRepository<BlogApp.API.Core.Entities.Comment>>();
        if (parentCommentId.HasValue)
        {
            mockCommentRepo.Setup(x => x.GetByIdAsync(parentCommentId.Value)).ReturnsAsync(parentComment);
        }
        mockCommentRepo.Setup(x => x.AddAsync(It.IsAny<BlogApp.API.Core.Entities.Comment>())).Returns(Task.CompletedTask);
        _mockUnitOfWork.Setup(x => x.Repository<BlogApp.API.Core.Entities.Comment>()).Returns(mockCommentRepo.Object);
    }

    [Fact]
    public async Task HandleAsync_WithValidData_ShouldReturnSuccessResponse()
    {
        // Arrange
        var command = CreateCommand();
        var blogPost = new BlogApp.API.Core.Entities.BlogPost { Id = 1, Title = "Test Post" };
        var commentEntity = new BlogApp.API.Core.Entities.Comment { Id = 10, Content = command.Content, BlogPostId = 1, AuthorId = "user-1", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow };
        var commentDto = new BlogApp.API.Application.Features.Comment.DTOs.CommentDTO(10, command.Content, "Author", "profile.jpg", null, commentEntity.CreatedAt);
        SetupBlogPostRepo(1, blogPost);
        SetupCommentRepo();
        _mockUnitOfWork.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);
        _mockOutboxService.Setup(x => x.AddAsync(nameof(BlogApp.API.Application.Features.Comment.Commands.CreateCommentCommand), command, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        _mockMapper.Setup(x => x.Map<BlogApp.API.Application.Features.Comment.DTOs.CommentDTO>(It.IsAny<BlogApp.API.Core.Entities.Comment>())).Returns(commentDto);
        // Act
        var result = await _handler.HandleAsync(command);
        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Content.Should().Be(command.Content);
        result.Message.Should().Be("Comment created successfully");
    }

    [Fact]
    public async Task HandleAsync_WithNonExistentBlogPost_ShouldReturnNotFound()
    {
        // Arrange
        var command = CreateCommand(blogPostId: 99);
        SetupBlogPostRepo(99, null);
        // Act
        var result = await _handler.HandleAsync(command);
        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(404);
        result.Message.Should().Contain("Blog post with ID 99 not found");
    }

    [Fact]
    public async Task HandleAsync_WithNonExistentParentComment_ShouldReturnNotFound()
    {
        // Arrange
        var command = CreateCommand(parentCommentId: 123);
        var blogPost = new BlogApp.API.Core.Entities.BlogPost { Id = 1, Title = "Test Post" };
        SetupBlogPostRepo(1, blogPost);
        SetupCommentRepo(parentCommentId: 123, parentComment: null);
        // Act
        var result = await _handler.HandleAsync(command);
        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(404);
        result.Message.Should().Contain("Parent comment with ID 123 not found");
    }
} 