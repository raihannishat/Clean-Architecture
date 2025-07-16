namespace BlogApp.UnitTests.Validators;

public class LoginCommandValidatorTests
{
    private readonly LoginCommandValidator _validator;

    public LoginCommandValidatorTests()
    {
        _validator = new LoginCommandValidator();
    }

    [Fact]
    public void Validate_WithValidData_ShouldPass()
    {
        // Arrange
        var command = new BlogApp.API.Application.Features.Auth.Commands.LoginCommand(
            "john@example.com",
            "password123"
        );

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public void Validate_WithEmptyEmail_ShouldFail()
    {
        // Arrange
        var command = new BlogApp.API.Application.Features.Auth.Commands.LoginCommand(
            "",
            "password123"
        );

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "Email" && e.ErrorMessage == "Email is required");
    }

    [Fact]
    public void Validate_WithInvalidEmailFormat_ShouldFail()
    {
        // Arrange
        var command = new BlogApp.API.Application.Features.Auth.Commands.LoginCommand(
            "invalid-email",
            "Password123!"
        );

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "Email" && e.ErrorMessage == "Invalid email format");
    }

    [Fact]
    public void Validate_WithEmptyPassword_ShouldFail()
    {
        // Arrange
        var command = new BlogApp.API.Application.Features.Auth.Commands.LoginCommand(
            "john@example.com",
            ""
        );

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "Password" && e.ErrorMessage == "Password is required");
    }

    [Fact]
    public void Validate_WithShortPassword_ShouldFail()
    {
        // Arrange
        var command = new BlogApp.API.Application.Features.Auth.Commands.LoginCommand(
            "test@example.com",
            "123"
        );

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "Password" && e.ErrorMessage == "Password must be at least 6 characters");
    }
} 