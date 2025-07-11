using BlogApp.API.Application.Features.Blog.Queries;
using BlogApp.API.Application.Common;
using BlogApp.API.Core.Entities;
using BlogApp.API.Infrastructure.Persistence.UnitOfWork.Interfaces;
using BlogApp.API.Infrastructure.Persistence.Repositories.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace BlogApp.UnitTests.Handlers;

public class SearchPostsQueryHandlerTests
{
    private readonly Mock<IUnitOfWorkFactory> _mockUnitOfWorkFactory;
    private readonly Mock<IQueryUnitOfWork> _mockUnitOfWork;
    private readonly SearchPostsQueryHandler _handler;

    public SearchPostsQueryHandlerTests()
    {
        _mockUnitOfWorkFactory = new Mock<IUnitOfWorkFactory>();
        _mockUnitOfWork = new Mock<IQueryUnitOfWork>();
        _mockUnitOfWorkFactory.Setup(x => x.CreateQueryUnitOfWork()).Returns(_mockUnitOfWork.Object);
        _handler = new SearchPostsQueryHandler(_mockUnitOfWorkFactory.Object);
    }

    [Fact]
    public async Task HandleAsync_WithValidSearchTerm_ShouldReturnSuccessResponse()
    {
        // Arrange
        var query = new SearchPostsQuery
        {
            SearchTerm = "technology",
            Page = 1,
            PageSize = 10
        };

        var blogPosts = new List<BlogPost>
        {
            new()
            {
                Id = 1,
                Title = "Technology Trends 2024",
                Content = "Latest technology trends in 2024",
                Slug = "technology-trends-2024",
                CategoryId = 1,
                AuthorId = "user-1",
                CreatedAt = DateTime.UtcNow.AddDays(-1)
            },
            new()
            {
                Id = 2,
                Title = "Modern Web Development",
                Content = "Modern web development with technology",
                Slug = "modern-web-development",
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
        result.Data[0].Title.Should().Contain("Technology");
        result.Data[1].Title.Should().Contain("Modern");
    }

    [Fact]
    public async Task HandleAsync_WithNoMatchingResults_ShouldReturnEmptyList()
    {
        // Arrange
        var query = new SearchPostsQuery
        {
            SearchTerm = "nonexistent",
            Page = 1,
            PageSize = 10
        };

        var blogPosts = new List<BlogPost>
        {
            new()
            {
                Id = 1,
                Title = "Technology Trends 2024",
                Content = "Latest technology trends in 2024",
                Slug = "technology-trends-2024",
                CategoryId = 1,
                AuthorId = "user-1",
                CreatedAt = DateTime.UtcNow.AddDays(-1)
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
        result.Data!.Count.Should().Be(0);
    }

    [Fact]
    public async Task HandleAsync_WithEmptySearchTerm_ShouldReturnAllPosts()
    {
        // Arrange
        var query = new SearchPostsQuery
        {
            SearchTerm = "",
            Page = 1,
            PageSize = 10
        };

        var blogPosts = new List<BlogPost>
        {
            new()
            {
                Id = 1,
                Title = "First Post",
                Content = "First content",
                Slug = "first-post",
                CategoryId = 1,
                AuthorId = "user-1",
                CreatedAt = DateTime.UtcNow.AddDays(-1)
            },
            new()
            {
                Id = 2,
                Title = "Second Post",
                Content = "Second content",
                Slug = "second-post",
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
    }

    [Fact]
    public async Task HandleAsync_WithInvalidPage_ShouldReturnValidationError()
    {
        // Arrange
        var query = new SearchPostsQuery
        {
            SearchTerm = "test",
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
        var query = new SearchPostsQuery
        {
            SearchTerm = "test",
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