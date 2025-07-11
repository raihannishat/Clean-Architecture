using BlogApp.API.Application.Features.Comment.Commands;
using FluentAssertions;
using Xunit;

namespace BlogApp.UnitTests.Validators;

public class CreateCommentCommandValidatorTests
{
    private readonly CreateCommentCommandValidator _validator;

    public CreateCommentCommandValidatorTests()
    {
        _validator = new CreateCommentCommandValidator();
    }

    [Fact]
    public void Validate_WithValidData_ShouldPass()
    {
        // Arrange
        var command = new CreateCommentCommand
        {
            Content = "This is a great article!",
            BlogPostId = 1,
            AuthorId = "user-id"
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public void Validate_WithEmptyContent_ShouldFail()
    {
        // Arrange
        var command = new CreateCommentCommand
        {
            Content = "",
            BlogPostId = 1,
            AuthorId = "user-id"
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == nameof(CreateCommentCommand.Content));
    }

    [Fact]
    public void Validate_WithNullContent_ShouldFail()
    {
        // Arrange
        var command = new CreateCommentCommand
        {
            Content = null!,
            BlogPostId = 1,
            AuthorId = "user-id"
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == nameof(CreateCommentCommand.Content));
    }

    [Fact]
    public void Validate_WithInvalidBlogPostId_ShouldFail()
    {
        // Arrange
        var command = new CreateCommentCommand
        {
            Content = "This is a great article!",
            BlogPostId = 0,
            AuthorId = "user-id"
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == nameof(CreateCommentCommand.BlogPostId));
    }

    [Fact]
    public void Validate_WithNegativeBlogPostId_ShouldFail()
    {
        // Arrange
        var command = new CreateCommentCommand
        {
            Content = "This is a great article!",
            BlogPostId = -1,
            AuthorId = "user-id"
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == nameof(CreateCommentCommand.BlogPostId));
    }

    [Fact]
    public void Validate_WithEmptyAuthorId_ShouldFail()
    {
        // Arrange
        var command = new CreateCommentCommand
        {
            Content = "This is a great article!",
            BlogPostId = 1,
            AuthorId = ""
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == nameof(CreateCommentCommand.AuthorId));
    }

    [Fact]
    public void Validate_WithNullAuthorId_ShouldFail()
    {
        // Arrange
        var command = new CreateCommentCommand
        {
            Content = "This is a great article!",
            BlogPostId = 1,
            AuthorId = null!
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == nameof(CreateCommentCommand.AuthorId));
    }

    [Fact]
    public void Validate_WithShortContent_ShouldFail()
    {
        // Arrange
        var command = new CreateCommentCommand
        {
            Content = "Hi",
            BlogPostId = 1,
            AuthorId = "user-id"
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == nameof(CreateCommentCommand.Content));
    }

    [Fact]
    public void Validate_WithLongContent_ShouldFail()
    {
        // Arrange
        var command = new CreateCommentCommand
        {
            Content = new string('a', 1001),
            BlogPostId = 1,
            AuthorId = "user-id"
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == nameof(CreateCommentCommand.Content));
    }
} 