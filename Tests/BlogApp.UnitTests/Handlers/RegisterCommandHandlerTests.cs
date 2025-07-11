using BlogApp.API.Application.Features.Auth.Commands;
using BlogApp.API.Application.Common;
using BlogApp.API.Core.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace BlogApp.UnitTests.Handlers;

public class RegisterCommandHandlerTests
{
    private readonly Mock<IAuthService> _mockAuthService;
    private readonly RegisterCommandHandler _handler;

    public RegisterCommandHandlerTests()
    {
        _mockAuthService = new Mock<IAuthService>();
        _handler = new RegisterCommandHandler(_mockAuthService.Object);
    }

    [Fact]
    public async Task HandleAsync_WithValidData_ShouldReturnSuccessResponse()
    {
        // Arrange
        var command = new RegisterCommand
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com",
            UserName = "johndoe",
            Password = "Password123!",
            ConfirmPassword = "Password123!"
        };

        var registerResponse = new RegisterResponse
        {
            UserId = "user-id",
            Email = "john.doe@example.com",
            UserName = "johndoe"
        };

        var authResult = BaseResponse<RegisterResponse>.Success(registerResponse, "Registration successful");

        _mockAuthService.Setup(x => x.RegisterAsync(
            command.FirstName, 
            command.LastName, 
            command.Email, 
            command.UserName, 
            command.Password, 
            command.ConfirmPassword))
            .ReturnsAsync(authResult);

        // Act
        var result = await _handler.HandleAsync(command);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Email.Should().Be(command.Email);
        result.Data.UserName.Should().Be(command.UserName);
    }

    [Fact]
    public async Task HandleAsync_WithRegistrationFailure_ShouldReturnFailureResponse()
    {
        // Arrange
        var command = new RegisterCommand
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com",
            UserName = "johndoe",
            Password = "Password123!",
            ConfirmPassword = "Password123!"
        };

        var authResult = BaseResponse<RegisterResponse>.Failure("Email already exists", 400);

        _mockAuthService.Setup(x => x.RegisterAsync(
            command.FirstName, 
            command.LastName, 
            command.Email, 
            command.UserName, 
            command.Password, 
            command.ConfirmPassword))
            .ReturnsAsync(authResult);

        // Act
        var result = await _handler.HandleAsync(command);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(400);
        result.Message.Should().Contain("Email already exists");
    }
} 