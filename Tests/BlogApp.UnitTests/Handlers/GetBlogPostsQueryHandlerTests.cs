using BlogApp.API.Application.Features.Blog.Queries;
using BlogApp.API.Application.Common;
using BlogApp.API.Core.Entities;
using BlogApp.API.Infrastructure.Persistence.UnitOfWork.Interfaces;
using BlogApp.API.Infrastructure.Persistence.Repositories.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace BlogApp.UnitTests.Handlers;

public class GetBlogPostsQueryHandlerTests
{
    private readonly Mock<IUnitOfWorkFactory> _mockUnitOfWorkFactory;
    private readonly Mock<IQueryUnitOfWork> _mockUnitOfWork;
    private readonly GetBlogPostsQueryHandler _handler;

    public GetBlogPostsQueryHandlerTests()
    {
        _mockUnitOfWorkFactory = new Mock<IUnitOfWorkFactory>();
        _mockUnitOfWork = new Mock<IQueryUnitOfWork>();
        _mockUnitOfWorkFactory.Setup(x => x.CreateQueryUnitOfWork()).Returns(_mockUnitOfWork.Object);
        _handler = new GetBlogPostsQueryHandler(_mockUnitOfWorkFactory.Object);
    }

    [Fact]
    public async Task HandleAsync_WithValidQuery_ShouldReturnSuccessResponse()
    {
        // Arrange
        var query = new GetBlogPostsQuery
        {
            Page = 1,
            PageSize = 10
        };

        var blogPosts = new List<BlogPost>
        {
            new()
            {
                Id = 1,
                Title = "First Blog Post",
                Content = "First content",
                Slug = "first-blog-post",
                CategoryId = 1,
                AuthorId = "user-1",
                CreatedAt = DateTime.UtcNow.AddDays(-1)
            },
            new()
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

        var mockBlogPostRepo = new Mock<IQueryRepository<BlogPost>>();
        mockBlogPostRepo.Setup(x => x.GetAllAsync())
            .ReturnsAsync(blogPosts);

        _mockUnitOfWork.Setup(x => x.Repository<BlogPost>()).Returns(mockBlogPostRepo.Object);

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
        var query = new GetBlogPostsQuery
        {
            Page = 2,
            PageSize = 5
        };

        var allBlogPosts = Enumerable.Range(1, 15)
            .Select(i => new BlogPost
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

        var mockBlogPostRepo = new Mock<IQueryRepository<BlogPost>>();
        mockBlogPostRepo.Setup(x => x.GetAllAsync())
            .ReturnsAsync(allBlogPosts);

        _mockUnitOfWork.Setup(x => x.Repository<BlogPost>()).Returns(mockBlogPostRepo.Object);

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
        var query = new GetBlogPostsQuery
        {
            Page = 1,
            PageSize = 10
        };

        var mockBlogPostRepo = new Mock<IQueryRepository<BlogPost>>();
        mockBlogPostRepo.Setup(x => x.GetAllAsync())
            .ReturnsAsync(new List<BlogPost>());

        _mockUnitOfWork.Setup(x => x.Repository<BlogPost>()).Returns(mockBlogPostRepo.Object);

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
        var query = new GetBlogPostsQuery
        {
            Page = 0,
            PageSize = 10
        };

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
        var query = new GetBlogPostsQuery
        {
            Page = 1,
            PageSize = 0
        };

        // Act
        var result = await _handler.HandleAsync(query);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(400);
        result.Errors.Should().Contain("Page size must be greater than 0");
    }
} 