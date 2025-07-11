using BlogApp.API.Application.Features.Comment.Queries;
using FluentAssertions;
using Xunit;

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
        var query = new GetCommentsQuery
        {
            BlogPostId = 1,
            IncludeReplies = true
        };

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
        var query = new GetCommentsQuery
        {
            BlogPostId = 0,
            IncludeReplies = true
        };

        // Act
        var result = _validator.Validate(query);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == nameof(GetCommentsQuery.BlogPostId));
    }

    [Fact]
    public void Validate_WithNegativeBlogPostId_ShouldFail()
    {
        // Arrange
        var query = new GetCommentsQuery
        {
            BlogPostId = -1,
            IncludeReplies = true
        };

        // Act
        var result = _validator.Validate(query);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == nameof(GetCommentsQuery.BlogPostId));
    }

    [Fact]
    public void Validate_WithValidLargeBlogPostId_ShouldPass()
    {
        // Arrange
        var query = new GetCommentsQuery
        {
            BlogPostId = int.MaxValue,
            IncludeReplies = false
        };

        // Act
        var result = _validator.Validate(query);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }
} 