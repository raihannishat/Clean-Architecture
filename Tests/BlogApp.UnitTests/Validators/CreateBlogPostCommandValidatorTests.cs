using BlogApp.API.Application.Features.Blog.Commands;
using FluentAssertions;
using Xunit;

namespace BlogApp.UnitTests.Validators;

public class CreateBlogPostCommandValidatorTests
{
    private readonly CreateBlogPostCommandValidator _validator;

    public CreateBlogPostCommandValidatorTests()
    {
        _validator = new CreateBlogPostCommandValidator();
    }

    [Fact]
    public void Validate_WithValidData_ShouldPass()
    {
        // Arrange
        var command = new CreateBlogPostCommand
        {
            Title = "Test Blog Post",
            Content = "This is a test blog post content.",
            Slug = "test-blog-post",
            CategoryId = 1,
            TagIds = new List<int> { 1, 2 },
            AuthorId = "user-id"
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public void Validate_WithEmptyTitle_ShouldFail()
    {
        // Arrange
        var command = new CreateBlogPostCommand
        {
            Title = "",
            Content = "This is a test blog post content.",
            Slug = "test-blog-post",
            CategoryId = 1,
            AuthorId = "user-id"
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "Title" && e.ErrorMessage == "Title is required");
    }

    [Fact]
    public void Validate_WithLongTitle_ShouldFail()
    {
        // Arrange
        var command = new CreateBlogPostCommand
        {
            Title = new string('A', 201),
            Content = "This is a test blog post content.",
            Slug = "test-blog-post",
            CategoryId = 1,
            AuthorId = "user-id"
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "Title" && e.ErrorMessage == "Title cannot exceed 200 characters");
    }

    [Fact]
    public void Validate_WithEmptyContent_ShouldFail()
    {
        // Arrange
        var command = new CreateBlogPostCommand
        {
            Title = "Test Blog Post",
            Content = "",
            Slug = "test-blog-post",
            CategoryId = 1,
            AuthorId = "user-id"
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "Content" && e.ErrorMessage == "Content is required");
    }

    [Fact]
    public void Validate_WithEmptySlug_ShouldFail()
    {
        // Arrange
        var command = new CreateBlogPostCommand
        {
            Title = "Test Blog Post",
            Content = "This is a test blog post content.",
            Slug = "",
            CategoryId = 1,
            AuthorId = "user-id"
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "Slug" && e.ErrorMessage == "Slug is required");
    }

    [Fact]
    public void Validate_WithInvalidSlug_ShouldFail()
    {
        // Arrange
        var command = new CreateBlogPostCommand
        {
            Title = "Test Blog Post",
            Content = "This is a test blog post content.",
            Slug = "invalid slug with spaces",
            CategoryId = 1,
            AuthorId = "user-id"
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "Slug" && e.ErrorMessage == "Slug must be URL-friendly");
    }

    [Fact]
    public void Validate_WithInvalidCategoryId_ShouldFail()
    {
        // Arrange
        var command = new CreateBlogPostCommand
        {
            Title = "Test Blog Post",
            Content = "This is a test blog post content.",
            Slug = "test-blog-post",
            CategoryId = 0,
            AuthorId = "user-id"
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "CategoryId" && e.ErrorMessage == "Category ID must be greater than 0");
    }

    [Fact]
    public void Validate_WithEmptyAuthorId_ShouldFail()
    {
        // Arrange
        var command = new CreateBlogPostCommand
        {
            Title = "Test Blog Post",
            Content = "This is a test blog post content.",
            Slug = "test-blog-post",
            CategoryId = 1,
            AuthorId = ""
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "AuthorId" && e.ErrorMessage == "Author ID is required");
    }
} 