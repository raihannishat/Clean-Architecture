namespace BlogApp.UnitTests.Validators;

public class GetBlogPostBySlugQueryValidatorTests
{
    private readonly GetBlogPostBySlugQueryValidator _validator;

    public GetBlogPostBySlugQueryValidatorTests()
    {
        _validator = new GetBlogPostBySlugQueryValidator();
    }

    [Fact]
    public void Validate_WithValidSlug_ShouldPass()
    {
        // Arrange
        var query = new BlogApp.API.Application.Features.Blog.Queries.GetBlogPostBySlugQuery("test-blog-post");

        // Act
        var result = _validator.Validate(query);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public void Validate_WithEmptySlug_ShouldFail()
    {
        // Arrange
        var query = new BlogApp.API.Application.Features.Blog.Queries.GetBlogPostBySlugQuery("");

        // Act
        var result = _validator.Validate(query);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == nameof(GetBlogPostBySlugQuery.Slug) && e.ErrorMessage == "Slug is required");
    }

    [Fact]
    public void Validate_WithNullSlug_ShouldFail()
    {
        // Arrange
        var query = new BlogApp.API.Application.Features.Blog.Queries.GetBlogPostBySlugQuery(null!);

        // Act
        var result = _validator.Validate(query);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == nameof(GetBlogPostBySlugQuery.Slug) && e.ErrorMessage == "Slug is required");
    }

    [Fact]
    public void Validate_WithWhitespaceSlug_ShouldFail()
    {
        // Arrange
        var query = new BlogApp.API.Application.Features.Blog.Queries.GetBlogPostBySlugQuery("   ");

        // Act
        var result = _validator.Validate(query);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == nameof(GetBlogPostBySlugQuery.Slug) && e.ErrorMessage == "Slug is required");
    }

    [Fact]
    public void Validate_WithShortSlug_ShouldFail()
    {
        // Arrange
        var query = new BlogApp.API.Application.Features.Blog.Queries.GetBlogPostBySlugQuery("ab");

        // Act
        var result = _validator.Validate(query);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == nameof(GetBlogPostBySlugQuery.Slug) && e.ErrorMessage == "Slug must be URL-friendly");
    }

    [Fact]
    public void Validate_WithLongSlug_ShouldFail()
    {
        // Arrange
        var query = new BlogApp.API.Application.Features.Blog.Queries.GetBlogPostBySlugQuery(new string('a', 101));

        // Act
        var result = _validator.Validate(query);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == nameof(GetBlogPostBySlugQuery.Slug) && e.ErrorMessage == "Slug must be URL-friendly");
    }

    [Fact]
    public void Validate_WithInvalidSlugFormat_ShouldFail()
    {
        // Arrange
        var query = new BlogApp.API.Application.Features.Blog.Queries.GetBlogPostBySlugQuery("Invalid Slug!");

        // Act
        var result = _validator.Validate(query);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == nameof(GetBlogPostBySlugQuery.Slug) && e.ErrorMessage == "Slug must be URL-friendly");
    }

    [Fact]
    public void Validate_WithValidSlugWithHyphens_ShouldPass()
    {
        // Arrange
        var query = new BlogApp.API.Application.Features.Blog.Queries.GetBlogPostBySlugQuery("test-blog-post-2024");

        // Act
        var result = _validator.Validate(query);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public void Validate_WithValidSlugWithNumbers_ShouldPass()
    {
        // Arrange
        var query = new BlogApp.API.Application.Features.Blog.Queries.GetBlogPostBySlugQuery("blog-post-123");

        // Act
        var result = _validator.Validate(query);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }
} 