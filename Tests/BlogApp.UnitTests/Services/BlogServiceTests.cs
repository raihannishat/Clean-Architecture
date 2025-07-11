using BlogApp.API.Application.Common;
using BlogApp.API.Core.Entities;
using BlogApp.API.Core.Interfaces;
using BlogApp.API.Infrastructure.Services;
using BlogApp.API.Infrastructure.Persistence.UnitOfWork.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace BlogApp.UnitTests.Services;

public class BlogServiceTests
{
    private readonly Mock<IBlogService> _mockBlogService;
    private readonly BlogService _blogService;

    public BlogServiceTests()
    {
        _mockBlogService = new Mock<IBlogService>();
        var mockUnitOfWork = new Mock<IQueryUnitOfWork>();
        _blogService = new BlogService(mockUnitOfWork.Object);
    }

    [Fact]
    public async Task GetBlogPostsAsync_WithValidParameters_ShouldReturnSuccessResponse()
    {
        // Arrange
        var page = 1;
        var pageSize = 10;

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

        var blogPost = new BlogPost
        {
            Id = 1,
            Title = "Test Blog Post",
            Content = "This is a test blog post content.",
            Slug = slug,
            CategoryId = 1,
            AuthorId = "user-id",
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            UpdatedAt = DateTime.UtcNow
        };

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
        var categories = new List<Category>
        {
            new()
            {
                Id = 1,
                Name = "Technology",
                Slug = "technology",
                IconClass = "fas fa-laptop",
                Color = "#007bff"
            },
            new()
            {
                Id = 2,
                Name = "Travel",
                Slug = "travel",
                IconClass = "fas fa-plane",
                Color = "#28a745"
            }
        };

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
        var tags = new List<Tag>
        {
            new()
            {
                Id = 1,
                Name = "C#",
                Slug = "csharp",
                IconClass = "fas fa-code",
                Color = "#007bff"
            },
            new()
            {
                Id = 2,
                Name = "ASP.NET",
                Slug = "aspnet",
                IconClass = "fas fa-server",
                Color = "#28a745"
            }
        };

        // Act
        var result = await _blogService.GetTagsAsync();

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
    }

    [Fact]
    public async Task CreateBlogPostAsync_WithValidData_ShouldReturnSuccessResponse()
    {
        // Arrange
        var blogPost = new BlogPost
        {
            Title = "New Blog Post",
            Content = "This is a new blog post content.",
            Slug = "new-blog-post",
            CategoryId = 1,
            AuthorId = "user-id"
        };

        // Act
        var result = await _blogService.CreateBlogPostAsync(blogPost);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Title.Should().Be("New Blog Post");
    }
} 