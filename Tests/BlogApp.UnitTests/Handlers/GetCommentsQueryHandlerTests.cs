namespace BlogApp.UnitTests.Handlers;

public class GetCommentsQueryHandlerTests
{
    private readonly Mock<IUnitOfWorkFactory> _mockUnitOfWorkFactory;
    private readonly Mock<IQueryUnitOfWork> _mockUnitOfWork;
    private readonly GetCommentsQueryHandler _handler;
    private readonly Mock<IAutoMapper> _mockMapper;

    public GetCommentsQueryHandlerTests()
    {
        _mockUnitOfWorkFactory = new Mock<IUnitOfWorkFactory>();
        _mockUnitOfWork = new Mock<IQueryUnitOfWork>();
        _mockMapper = new Mock<IAutoMapper>();
        _mockUnitOfWorkFactory.Setup(x => x.CreateQueryUnitOfWork()).Returns(_mockUnitOfWork.Object);
        _handler = new BlogApp.API.Application.Features.Comment.Queries.GetCommentsQueryHandler(_mockUnitOfWorkFactory.Object, _mockMapper.Object);
    }

    [Fact]
    public async Task HandleAsync_WithValidBlogPostId_ShouldReturnSuccessResponse()
    {
        // Arrange
        var query = new BlogApp.API.Application.Features.Comment.Queries.GetCommentsQuery(1, true);

        var comments = new List<BlogApp.API.Core.Entities.Comment>
        {
            new BlogApp.API.Core.Entities.Comment
            {
                Id = 1,
                Content = "Great article!",
                BlogPostId = 1,
                AuthorId = "user-1",
                CreatedAt = DateTime.UtcNow.AddDays(-1)
            },
            new BlogApp.API.Core.Entities.Comment
            {
                Id = 2,
                Content = "Very informative post.",
                BlogPostId = 1,
                AuthorId = "user-2",
                CreatedAt = DateTime.UtcNow
            }
        };

        var mockCommentRepo = new Mock<BlogApp.API.Infrastructure.Persistence.Repositories.Interfaces.IQueryRepository<BlogApp.API.Core.Entities.Comment>>();
        mockCommentRepo.Setup(x => x.GetAllAsync())
            .ReturnsAsync(comments);

        _mockUnitOfWork.Setup(x => x.Repository<BlogApp.API.Core.Entities.Comment>()).Returns(mockCommentRepo.Object);
        _mockMapper.Setup(x => x.Map<BlogApp.API.Application.Features.Comment.DTOs.CommentDTO>(It.IsAny<BlogApp.API.Core.Entities.Comment>()))
            .Returns((BlogApp.API.Core.Entities.Comment c) => new BlogApp.API.Application.Features.Comment.DTOs.CommentDTO(
                c.Id,
                c.Content,
                "Test Author", // AuthorName
                "profile.jpg", // AuthorProfileImageUrl
                null, // ParentCommentId
                c.CreatedAt
            ));

        // Act
        var result = await _handler.HandleAsync(query);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Count.Should().Be(2);
        result.Data[0].Content.Should().Be("Great article!");
        result.Data[1].Content.Should().Be("Very informative post.");
    }

    [Fact]
    public async Task HandleAsync_WithNoComments_ShouldReturnEmptyList()
    {
        // Arrange
        var query = new BlogApp.API.Application.Features.Comment.Queries.GetCommentsQuery(1, true);

        var mockCommentRepo = new Mock<BlogApp.API.Infrastructure.Persistence.Repositories.Interfaces.IQueryRepository<BlogApp.API.Core.Entities.Comment>>();
        mockCommentRepo.Setup(x => x.GetAllAsync())
            .ReturnsAsync(new List<BlogApp.API.Core.Entities.Comment>());

        _mockUnitOfWork.Setup(x => x.Repository<BlogApp.API.Core.Entities.Comment>()).Returns(mockCommentRepo.Object);
        _mockMapper.Setup(x => x.Map<BlogApp.API.Application.Features.Comment.DTOs.CommentDTO>(It.IsAny<BlogApp.API.Core.Entities.Comment>()))
            .Returns((BlogApp.API.Core.Entities.Comment c) => new BlogApp.API.Application.Features.Comment.DTOs.CommentDTO(
                c.Id,
                c.Content,
                "Test Author", // AuthorName
                "profile.jpg", // AuthorProfileImageUrl
                null, // ParentCommentId
                c.CreatedAt
            ));

        // Act
        var result = await _handler.HandleAsync(query);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Count.Should().Be(0);
    }

    [Fact]
    public async Task HandleAsync_WithInvalidBlogPostId_ShouldReturnValidationError()
    {
        // Arrange
        var query = new BlogApp.API.Application.Features.Comment.Queries.GetCommentsQuery(0, true);

        // Act
        var result = await _handler.HandleAsync(query);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(400);
        result.Errors.Should().Contain("Blog post ID must be greater than 0");
    }
} 