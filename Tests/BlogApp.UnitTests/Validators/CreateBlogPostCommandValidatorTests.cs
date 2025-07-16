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
        var command = new BlogApp.API.Application.Features.Blog.Commands.CreateBlogPostCommand(
            "Test Blog Post",
            "This is a test blog post content.",
            "test-blog-post",
            1,
            "user-id",
            new List<int> { 1, 2 }
        );

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
        var command = new BlogApp.API.Application.Features.Blog.Commands.CreateBlogPostCommand(
            "",
            "This is a test blog post content.",
            "test-blog-post",
            1,
            "user-id",
            new List<int>()
        );

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
        var command = new BlogApp.API.Application.Features.Blog.Commands.CreateBlogPostCommand(
            new string('A', 201),
            "This is a test blog post content.",
            "test-blog-post",
            1,
            "user-id",
            new List<int>()
        );

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
        var command = new BlogApp.API.Application.Features.Blog.Commands.CreateBlogPostCommand(
            "Test Blog Post",
            "",
            "test-blog-post",
            1,
            "user-id",
            new List<int>()
        );

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
        var command = new BlogApp.API.Application.Features.Blog.Commands.CreateBlogPostCommand(
            "Test Blog Post",
            "This is a test blog post content.",
            "",
            1,
            "user-id",
            new List<int>()
        );

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
        var command = new BlogApp.API.Application.Features.Blog.Commands.CreateBlogPostCommand(
            "Test Blog Post",
            "This is a test blog post content.",
            "invalid slug with spaces",
            1,
            "user-id",
            new List<int>()
        );

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
        var command = new BlogApp.API.Application.Features.Blog.Commands.CreateBlogPostCommand(
            "Test Blog Post",
            "This is a test blog post content.",
            "test-blog-post",
            0,
            "user-id",
            new List<int>()
        );

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
        var command = new BlogApp.API.Application.Features.Blog.Commands.CreateBlogPostCommand(
            "Test Blog Post",
            "This is a test blog post content.",
            "test-blog-post",
            1,
            "",
            new List<int>()
        );

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "AuthorId" && e.ErrorMessage == "Author ID is required");
    }
} 