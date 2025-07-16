namespace BlogApp.UnitTests.Validators;

public class CreateCommentCommandValidatorTests
{
    private readonly BlogApp.API.Application.Features.Comment.Validators.CreateCommentCommandValidator _validator;

    public CreateCommentCommandValidatorTests()
    {
        _validator = new BlogApp.API.Application.Features.Comment.Validators.CreateCommentCommandValidator();
    }

    [Fact]
    public void Validate_WithValidData_ShouldPass()
    {
        // Arrange
        var command = new BlogApp.API.Application.Features.Comment.Commands.CreateCommentCommand(
            "This is a comment.",
            1,
            "user-1",
            null
        );
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
        var command = new BlogApp.API.Application.Features.Comment.Commands.CreateCommentCommand(
            "",
            1,
            "user-1",
            null
        );
        // Act
        var result = _validator.Validate(command);
        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == nameof(command.Content) && e.ErrorMessage == "Comment content is required");
    }

    [Fact]
    public void Validate_WithLongContent_ShouldFail()
    {
        // Arrange
        var command = new BlogApp.API.Application.Features.Comment.Commands.CreateCommentCommand(
            new string('A', 1001),
            1,
            "user-1",
            null
        );
        // Act
        var result = _validator.Validate(command);
        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == nameof(command.Content) && e.ErrorMessage == "Comment content cannot exceed 1000 characters");
    }

    [Fact]
    public void Validate_WithInvalidBlogPostId_ShouldFail()
    {
        // Arrange
        var command = new BlogApp.API.Application.Features.Comment.Commands.CreateCommentCommand(
            "This is a comment.",
            0,
            "user-1",
            null
        );
        // Act
        var result = _validator.Validate(command);
        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == nameof(command.BlogPostId) && e.ErrorMessage == "Blog post ID must be greater than 0");
    }

    [Fact]
    public void Validate_WithEmptyAuthorId_ShouldFail()
    {
        // Arrange
        var command = new BlogApp.API.Application.Features.Comment.Commands.CreateCommentCommand(
            "This is a comment.",
            1,
            "",
            null
        );
        // Act
        var result = _validator.Validate(command);
        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == nameof(command.AuthorId) && e.ErrorMessage == "Author ID is required");
    }

    [Fact]
    public void Validate_WithInvalidParentCommentId_ShouldFail()
    {
        // Arrange
        var command = new BlogApp.API.Application.Features.Comment.Commands.CreateCommentCommand(
            "This is a comment.",
            1,
            "user-1",
            0
        );
        // Act
        var result = _validator.Validate(command);
        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == nameof(command.ParentCommentId) && e.ErrorMessage == "Parent comment ID must be greater than 0");
    }
} 