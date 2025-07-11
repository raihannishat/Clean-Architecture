using BlogApp.API.Application.Features.Blog.Queries;
using FluentAssertions;
using Xunit;

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
        var query = new GetBlogPostBySlugQuery
        {
            Slug = "test-blog-post"
        };

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
        var query = new GetBlogPostBySlugQuery
        {
            Slug = ""
        };

        // Act
        var result = _validator.Validate(query);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == nameof(GetBlogPostBySlugQuery.Slug));
    }

    [Fact]
    public void Validate_WithNullSlug_ShouldFail()
    {
        // Arrange
        var query = new GetBlogPostBySlugQuery
        {
            Slug = null!
        };

        // Act
        var result = _validator.Validate(query);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == nameof(GetBlogPostBySlugQuery.Slug));
    }

    [Fact]
    public void Validate_WithWhitespaceSlug_ShouldFail()
    {
        // Arrange
        var query = new GetBlogPostBySlugQuery
        {
            Slug = "   "
        };

        // Act
        var result = _validator.Validate(query);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == nameof(GetBlogPostBySlugQuery.Slug));
    }

    [Fact]
    public void Validate_WithShortSlug_ShouldFail()
    {
        // Arrange
        var query = new GetBlogPostBySlugQuery
        {
            Slug = "ab"
        };

        // Act
        var result = _validator.Validate(query);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == nameof(GetBlogPostBySlugQuery.Slug));
    }

    [Fact]
    public void Validate_WithLongSlug_ShouldFail()
    {
        // Arrange
        var query = new GetBlogPostBySlugQuery
        {
            Slug = new string('a', 101)
        };

        // Act
        var result = _validator.Validate(query);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == nameof(GetBlogPostBySlugQuery.Slug));
    }

    [Fact]
    public void Validate_WithInvalidSlugFormat_ShouldFail()
    {
        // Arrange
        var query = new GetBlogPostBySlugQuery
        {
            Slug = "invalid slug format"
        };

        // Act
        var result = _validator.Validate(query);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == nameof(GetBlogPostBySlugQuery.Slug));
    }

    [Fact]
    public void Validate_WithValidSlugWithHyphens_ShouldPass()
    {
        // Arrange
        var query = new GetBlogPostBySlugQuery
        {
            Slug = "test-blog-post-2024"
        };

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
        var query = new GetBlogPostBySlugQuery
        {
            Slug = "blog-post-123"
        };

        // Act
        var result = _validator.Validate(query);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }
} 