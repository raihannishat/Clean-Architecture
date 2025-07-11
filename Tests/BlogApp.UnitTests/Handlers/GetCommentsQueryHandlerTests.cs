using BlogApp.API.Application.Features.Comment.Queries;
using BlogApp.API.Application.Common;
using BlogApp.API.Core.Entities;
using BlogApp.API.Infrastructure.Persistence.UnitOfWork.Interfaces;
using BlogApp.API.Infrastructure.Persistence.Repositories.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace BlogApp.UnitTests.Handlers;

public class GetCommentsQueryHandlerTests
{
    private readonly Mock<IUnitOfWorkFactory> _mockUnitOfWorkFactory;
    private readonly Mock<IQueryUnitOfWork> _mockUnitOfWork;
    private readonly GetCommentsQueryHandler _handler;

    public GetCommentsQueryHandlerTests()
    {
        _mockUnitOfWorkFactory = new Mock<IUnitOfWorkFactory>();
        _mockUnitOfWork = new Mock<IQueryUnitOfWork>();
        _mockUnitOfWorkFactory.Setup(x => x.CreateQueryUnitOfWork()).Returns(_mockUnitOfWork.Object);
        _handler = new GetCommentsQueryHandler(_mockUnitOfWorkFactory.Object);
    }

    [Fact]
    public async Task HandleAsync_WithValidBlogPostId_ShouldReturnSuccessResponse()
    {
        // Arrange
        var query = new GetCommentsQuery
        {
            BlogPostId = 1,
            IncludeReplies = true
        };

        var comments = new List<Comment>
        {
            new()
            {
                Id = 1,
                Content = "Great article!",
                BlogPostId = 1,
                AuthorId = "user-1",
                CreatedAt = DateTime.UtcNow.AddDays(-1)
            },
            new()
            {
                Id = 2,
                Content = "Very informative post.",
                BlogPostId = 1,
                AuthorId = "user-2",
                CreatedAt = DateTime.UtcNow
            }
        };

        var mockCommentRepo = new Mock<IQueryRepository<Comment>>();
        mockCommentRepo.Setup(x => x.GetAllAsync())
            .ReturnsAsync(comments);

        _mockUnitOfWork.Setup(x => x.Repository<Comment>()).Returns(mockCommentRepo.Object);

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
        var query = new GetCommentsQuery
        {
            BlogPostId = 1,
            IncludeReplies = true
        };

        var mockCommentRepo = new Mock<IQueryRepository<Comment>>();
        mockCommentRepo.Setup(x => x.GetAllAsync())
            .ReturnsAsync(new List<Comment>());

        _mockUnitOfWork.Setup(x => x.Repository<Comment>()).Returns(mockCommentRepo.Object);

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
        var query = new GetCommentsQuery
        {
            BlogPostId = 0,
            IncludeReplies = true
        };

        // Act
        var result = await _handler.HandleAsync(query);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(400);
        result.Errors.Should().Contain("Blog post ID must be greater than 0");
    }
} 