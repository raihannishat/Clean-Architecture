namespace BlogApp.UnitTests.Validators;

public class RegisterCommandValidatorTests
{
    private readonly RegisterCommandValidator _validator;

    public RegisterCommandValidatorTests()
    {
        _validator = new RegisterCommandValidator();
    }

    [Fact]
    public void Validate_WithValidData_ShouldPass()
    {
        // Arrange
        var command = new BlogApp.API.Application.Features.Auth.Commands.RegisterCommand(
            "John",
            "Doe",
            "john@example.com",
            "johndoe",
            "Password123!",
            "Password123!"
        );

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public void Validate_WithEmptyFirstName_ShouldFail()
    {
        // Arrange
        var command = new BlogApp.API.Application.Features.Auth.Commands.RegisterCommand(
            "",
            "Doe",
            "john@example.com",
            "johndoe",
            "password123",
            "password123"
        );

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == nameof(RegisterCommand.FirstName) && e.ErrorMessage == "First name is required");
    }

    [Fact]
    public void Validate_WithEmptyLastName_ShouldFail()
    {
        // Arrange
        var command = new BlogApp.API.Application.Features.Auth.Commands.RegisterCommand(
            "John",
            "",
            "john@example.com",
            "johndoe",
            "password123",
            "password123"
        );

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == nameof(RegisterCommand.LastName) && e.ErrorMessage == "Last name is required");
    }

    [Fact]
    public void Validate_WithInvalidEmail_ShouldFail()
    {
        // Arrange
        var command = new RegisterCommand(
            "John",
            "Doe",
            "invalid-email",
            "johndoe",
            "Password123!",
            "Password123!"
        );

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == nameof(RegisterCommand.Email) && e.ErrorMessage == "Invalid email format");
    }

    [Fact]
    public void Validate_WithEmptyUserName_ShouldFail()
    {
        // Arrange
        var command = new BlogApp.API.Application.Features.Auth.Commands.RegisterCommand(
            "John",
            "Doe",
            "john@example.com",
            "",
            "password123",
            "password123"
        );

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == nameof(RegisterCommand.UserName) && e.ErrorMessage == "Username is required");
    }

    [Fact]
    public void Validate_WithWeakPassword_ShouldFail()
    {
        // Arrange
        var command = new RegisterCommand(
            "John",
            "Doe",
            "john.doe@example.com",
            "johndoe",
            "weak",
            "weak"
        );

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == nameof(RegisterCommand.Password) && e.ErrorMessage == "Password must be at least 6 characters");
    }

    [Fact]
    public void Validate_WithMismatchedPasswords_ShouldFail()
    {
        // Arrange
        var command = new RegisterCommand(
            "John",
            "Doe",
            "john.doe@example.com",
            "johndoe",
            "Password123!",
            "DifferentPassword123!"
        );

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == nameof(RegisterCommand.ConfirmPassword) && e.ErrorMessage == "Passwords do not match");
    }

    [Fact]
    public void Validate_WithEmptyPassword_ShouldFail()
    {
        // Arrange
        var command = new BlogApp.API.Application.Features.Auth.Commands.RegisterCommand(
            "John",
            "Doe",
            "john@example.com",
            "johndoe",
            "",
            "password123"
        );

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == nameof(RegisterCommand.Password) && e.ErrorMessage == "Password must be at least 6 characters");
    }

    [Fact]
    public void Validate_WithEmptyConfirmPassword_ShouldFail()
    {
        // Arrange
        var command = new BlogApp.API.Application.Features.Auth.Commands.RegisterCommand(
            "John",
            "Doe",
            "john@example.com",
            "johndoe",
            "password123",
            ""
        );

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == nameof(RegisterCommand.ConfirmPassword) && e.ErrorMessage == "Passwords do not match");
    }
} 