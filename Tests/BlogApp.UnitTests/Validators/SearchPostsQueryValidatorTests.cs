using BlogApp.API.Application.Features.Blog.Queries;
using FluentAssertions;
using Xunit;

namespace BlogApp.UnitTests.Validators;

public class SearchPostsQueryValidatorTests
{
    private readonly SearchPostsQueryValidator _validator;

    public SearchPostsQueryValidatorTests()
    {
        _validator = new SearchPostsQueryValidator();
    }

    [Fact]
    public void Validate_WithValidData_ShouldPass()
    {
        // Arrange
        var query = new SearchPostsQuery
        {
            SearchTerm = "technology",
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
    public void Validate_WithEmptySearchTerm_ShouldPass()
    {
        // Arrange
        var query = new SearchPostsQuery
        {
            SearchTerm = "",
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
    public void Validate_WithNullSearchTerm_ShouldPass()
    {
        // Arrange
        var query = new SearchPostsQuery
        {
            SearchTerm = null,
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
        var query = new SearchPostsQuery
        {
            SearchTerm = "technology",
            Page = 0,
            PageSize = 10
        };

        // Act
        var result = _validator.Validate(query);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == nameof(SearchPostsQuery.Page));
    }

    [Fact]
    public void Validate_WithNegativePage_ShouldFail()
    {
        // Arrange
        var query = new SearchPostsQuery
        {
            SearchTerm = "technology",
            Page = -1,
            PageSize = 10
        };

        // Act
        var result = _validator.Validate(query);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == nameof(SearchPostsQuery.Page));
    }

    [Fact]
    public void Validate_WithInvalidPageSize_ShouldFail()
    {
        // Arrange
        var query = new SearchPostsQuery
        {
            SearchTerm = "technology",
            Page = 1,
            PageSize = 0
        };

        // Act
        var result = _validator.Validate(query);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == nameof(SearchPostsQuery.PageSize));
    }

    [Fact]
    public void Validate_WithNegativePageSize_ShouldFail()
    {
        // Arrange
        var query = new SearchPostsQuery
        {
            SearchTerm = "technology",
            Page = 1,
            PageSize = -1
        };

        // Act
        var result = _validator.Validate(query);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == nameof(SearchPostsQuery.PageSize));
    }

    [Fact]
    public void Validate_WithLargePageSize_ShouldFail()
    {
        // Arrange
        var query = new SearchPostsQuery
        {
            SearchTerm = "technology",
            Page = 1,
            PageSize = 101
        };

        // Act
        var result = _validator.Validate(query);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == nameof(SearchPostsQuery.PageSize));
    }

    [Fact]
    public void Validate_WithLongSearchTerm_ShouldFail()
    {
        // Arrange
        var query = new SearchPostsQuery
        {
            SearchTerm = new string('a', 101),
            Page = 1,
            PageSize = 10
        };

        // Act
        var result = _validator.Validate(query);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == nameof(SearchPostsQuery.SearchTerm));
    }

    [Fact]
    public void Validate_WithDefaultValues_ShouldPass()
    {
        // Arrange
        var query = new SearchPostsQuery();

        // Act
        var result = _validator.Validate(query);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }
} 