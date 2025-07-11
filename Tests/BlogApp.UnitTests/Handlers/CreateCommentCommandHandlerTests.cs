using BlogApp.API.Application.Features.Comment.Commands;
using BlogApp.API.Core.Entities;
using BlogApp.API.Infrastructure.Persistence.UnitOfWork.Interfaces;
using BlogApp.API.Infrastructure.Persistence.Repositories.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace BlogApp.UnitTests.Handlers;

public class CreateCommentCommandHandlerTests
{
    private readonly Mock<IUnitOfWorkFactory> _mockUnitOfWorkFactory;
    private readonly Mock<ICommandUnitOfWork> _mockUnitOfWork;
    private readonly CreateCommentCommandHandler _handler;

    public CreateCommentCommandHandlerTests()
    {
        _mockUnitOfWorkFactory = new Mock<IUnitOfWorkFactory>();
        _mockUnitOfWork = new Mock<ICommandUnitOfWork>();
        _mockUnitOfWorkFactory.Setup(x => x.CreateCommandUnitOfWork()).Returns(_mockUnitOfWork.Object);
        _handler = new CreateCommentCommandHandler(_mockUnitOfWorkFactory.Object);
    }

    [Fact]
    public async Task HandleAsync_WithValidData_ShouldReturnSuccessResponse()
    {
        // Arrange
        var command = new CreateCommentCommand
        {
            Content = "This is a test comment.",
            BlogPostId = 1,
            AuthorId = "user-id"
        };

        var blogPost = new BlogPost
        {
            Id = 1,
            Title = "Test Blog Post",
            Content = "Test content"
        };

        var mockBlogPostRepo = new Mock<ICommandRepository<BlogPost>>();
        var mockCommentRepo = new Mock<ICommandRepository<Comment>>();

        mockBlogPostRepo.Setup(x => x.GetByIdAsync(command.BlogPostId))
            .ReturnsAsync(blogPost);

        mockCommentRepo.Setup(x => x.AddAsync(It.IsAny<Comment>()))
            .Returns(Task.CompletedTask);

        _mockUnitOfWork.Setup(x => x.Repository<BlogPost>()).Returns(mockBlogPostRepo.Object);
        _mockUnitOfWork.Setup(x => x.Repository<Comment>()).Returns(mockCommentRepo.Object);
        _mockUnitOfWork.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        var result = await _handler.HandleAsync(command);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Content.Should().Be(command.Content);
        result.Data.BlogPostId.Should().Be(command.BlogPostId);
        result.Data.AuthorId.Should().Be(command.AuthorId);
    }

    [Fact]
    public async Task HandleAsync_WithNonExistentBlogPost_ShouldReturnNotFoundResponse()
    {
        // Arrange
        var command = new CreateCommentCommand
        {
            Content = "This is a test comment.",
            BlogPostId = 999,
            AuthorId = "user-id"
        };

        var mockBlogPostRepo = new Mock<ICommandRepository<BlogPost>>();
        mockBlogPostRepo.Setup(x => x.GetByIdAsync(command.BlogPostId))
            .ReturnsAsync((BlogPost?)null);

        _mockUnitOfWork.Setup(x => x.Repository<BlogPost>()).Returns(mockBlogPostRepo.Object);

        // Act
        var result = await _handler.HandleAsync(command);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(404);
        result.Message.Should().Contain("Blog post not found");
    }

    [Fact]
    public async Task HandleAsync_WithEmptyContent_ShouldReturnValidationError()
    {
        // Arrange
        var command = new CreateCommentCommand
        {
            Content = "",
            BlogPostId = 1,
            AuthorId = "user-id"
        };

        // Act
        var result = await _handler.HandleAsync(command);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(400);
        result.Errors.Should().Contain("Content is required");
    }

    [Fact]
    public async Task HandleAsync_WithInvalidBlogPostId_ShouldReturnValidationError()
    {
        // Arrange
        var command = new CreateCommentCommand
        {
            Content = "This is a test comment.",
            BlogPostId = 0,
            AuthorId = "user-id"
        };

        // Act
        var result = await _handler.HandleAsync(command);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(400);
        result.Errors.Should().Contain("Blog post ID must be greater than 0");
    }
} 