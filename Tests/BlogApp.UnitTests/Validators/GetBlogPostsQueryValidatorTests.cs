using BlogApp.API.Application.Features.Blog.Queries;
using FluentAssertions;
using Xunit;

namespace BlogApp.UnitTests.Validators;

public class GetBlogPostsQueryValidatorTests
{
    private readonly GetBlogPostsQueryValidator _validator;

    public GetBlogPostsQueryValidatorTests()
    {
        _validator = new GetBlogPostsQueryValidator();
    }

    [Fact]
    public void Validate_WithValidData_ShouldPass()
    {
        // Arrange
        var query = new GetBlogPostsQuery
        {
            Page = 1,
            PageSize = 10
        };

        // Act
        var result = _validator.Validate(query);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public void Validate_WithInvalidPage_ShouldFail()
    {
        // Arrange
        var query = new GetBlogPostsQuery
        {
            Page = 0,
            PageSize = 10
        };

        // Act
        var result = _validator.Validate(query);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == nameof(GetBlogPostsQuery.Page));
    }

    [Fact]
    public void Validate_WithNegativePage_ShouldFail()
    {
        // Arrange
        var query = new GetBlogPostsQuery
        {
            Page = -1,
            PageSize = 10
        };

        // Act
        var result = _validator.Validate(query);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == nameof(GetBlogPostsQuery.Page));
    }

    [Fact]
    public void Validate_WithInvalidPageSize_ShouldFail()
    {
        // Arrange
        var query = new GetBlogPostsQuery
        {
            Page = 1,
            PageSize = 0
        };

        // Act
        var result = _validator.Validate(query);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == nameof(GetBlogPostsQuery.PageSize));
    }

    [Fact]
    public void Validate_WithNegativePageSize_ShouldFail()
    {
        // Arrange
        var query = new GetBlogPostsQuery
        {
            Page = 1,
            PageSize = -1
        };

        // Act
        var result = _validator.Validate(query);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == nameof(GetBlogPostsQuery.PageSize));
    }

    [Fact]
    public void Validate_WithLargePageSize_ShouldFail()
    {
        // Arrange
        var query = new GetBlogPostsQuery
        {
            Page = 1,
            PageSize = 101
        };

        // Act
        var result = _validator.Validate(query);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == nameof(GetBlogPostsQuery.PageSize));
    }

    [Fact]
    public void Validate_WithDefaultValues_ShouldPass()
    {
        // Arrange
        var query = new GetBlogPostsQuery();

        // Act
        var result = _validator.Validate(query);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }
} 