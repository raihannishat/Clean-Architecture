namespace BlogApp.UnitTests.Validators;

public class SearchPostsQueryValidatorTests
{
    private readonly SearchPostsQueryValidator _validator;

    public SearchPostsQueryValidatorTests()
    {
        _validator = new SearchPostsQueryValidator();
    }

    [Fact]
    public void Validate_WithEmptySearchTerm_ShouldFail()
    {
        // Arrange
        var query = new BlogApp.API.Application.Features.Blog.Queries.SearchPostsQuery("", 1, 10, false);

        // Act
        var result = _validator.Validate(query);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == nameof(SearchPostsQuery.SearchTerm) && e.ErrorMessage == "Search term is required");
    }

    [Fact]
    public void Validate_WithNullSearchTerm_ShouldFail()
    {
        // Arrange
        var query = new BlogApp.API.Application.Features.Blog.Queries.SearchPostsQuery(null!, 1, 10, false);

        // Act
        var result = _validator.Validate(query);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == nameof(SearchPostsQuery.SearchTerm) && e.ErrorMessage == "Search term is required");
    }

    [Fact]
    public void Validate_WithLongSearchTerm_ShouldFail()
    {
        // Arrange
        var query = new BlogApp.API.Application.Features.Blog.Queries.SearchPostsQuery(new string('a', 201), 1, 10, false);

        // Act
        var result = _validator.Validate(query);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == nameof(SearchPostsQuery.SearchTerm) && e.ErrorMessage == "Search term cannot exceed 200 characters");
    }

    [Fact]
    public void Validate_WithInvalidPage_ShouldFail()
    {
        // Arrange
        var query = new BlogApp.API.Application.Features.Blog.Queries.SearchPostsQuery("test", 0, 10, false);

        // Act
        var result = _validator.Validate(query);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == nameof(SearchPostsQuery.Page) && e.ErrorMessage == "Page must be greater than 0");
    }

    [Fact]
    public void Validate_WithNegativePage_ShouldFail()
    {
        // Arrange
        var query = new BlogApp.API.Application.Features.Blog.Queries.SearchPostsQuery("test", -1, 10, false);

        // Act
        var result = _validator.Validate(query);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == nameof(SearchPostsQuery.Page) && e.ErrorMessage == "Page must be greater than 0");
    }

    [Fact]
    public void Validate_WithInvalidPageSize_ShouldFail()
    {
        // Arrange
        var query = new BlogApp.API.Application.Features.Blog.Queries.SearchPostsQuery("test", 1, 0, false);

        // Act
        var result = _validator.Validate(query);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == nameof(SearchPostsQuery.PageSize) && e.ErrorMessage == "Page size must be greater than 0");
    }

    [Fact]
    public void Validate_WithLargePageSize_ShouldFail()
    {
        // Arrange
        var query = new BlogApp.API.Application.Features.Blog.Queries.SearchPostsQuery("test", 1, 101, false);

        // Act
        var result = _validator.Validate(query);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == nameof(SearchPostsQuery.PageSize) && e.ErrorMessage == "Page size cannot exceed 100");
    }

    [Fact]
    public void Validate_WithValidData_ShouldPass()
    {
        // Arrange
        var query = new BlogApp.API.Application.Features.Blog.Queries.SearchPostsQuery("test", 1, 10, false);

        // Act
        var result = _validator.Validate(query);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }
} 