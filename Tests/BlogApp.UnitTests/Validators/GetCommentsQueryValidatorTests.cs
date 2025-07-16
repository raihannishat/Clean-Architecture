namespace BlogApp.UnitTests.Validators;

public class GetCommentsQueryValidatorTests
{
    private readonly GetCommentsQueryValidator _validator;

    public GetCommentsQueryValidatorTests()
    {
        _validator = new GetCommentsQueryValidator();
    }

    [Fact]
    public void Validate_WithValidData_ShouldPass()
    {
        // Arrange
        var query = new BlogApp.API.Application.Features.Comment.Queries.GetCommentsQuery(1, true);

        // Act
        var result = _validator.Validate(query);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public void Validate_WithInvalidBlogPostId_ShouldFail()
    {
        // Arrange
        var query = new BlogApp.API.Application.Features.Comment.Queries.GetCommentsQuery(0, true);

        // Act
        var result = _validator.Validate(query);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == nameof(GetCommentsQuery.BlogPostId) && e.ErrorMessage == "Blog post ID must be greater than 0");
    }

    [Fact]
    public void Validate_WithNegativeBlogPostId_ShouldFail()
    {
        // Arrange
        var query = new BlogApp.API.Application.Features.Comment.Queries.GetCommentsQuery(-1, true);

        // Act
        var result = _validator.Validate(query);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == nameof(GetCommentsQuery.BlogPostId) && e.ErrorMessage == "Blog post ID must be greater than 0");
    }

    [Fact]
    public void Validate_WithDefaultValues_ShouldPass()
    {
        // Arrange
        var query = new BlogApp.API.Application.Features.Comment.Queries.GetCommentsQuery(1, false);

        // Act
        var result = _validator.Validate(query);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }
} 