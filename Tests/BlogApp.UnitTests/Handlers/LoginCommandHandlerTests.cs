using BlogApp.API.Application.Features.Auth.Commands;
using BlogApp.API.Application.Common;
using BlogApp.API.Core.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace BlogApp.UnitTests.Handlers;

public class LoginCommandHandlerTests
{
    private readonly Mock<IAuthService> _mockAuthService;
    private readonly LoginCommandHandler _handler;

    public LoginCommandHandlerTests()
    {
        _mockAuthService = new Mock<IAuthService>();
        _handler = new LoginCommandHandler(_mockAuthService.Object);
    }

    [Fact]
    public async Task HandleAsync_WithValidCredentials_ShouldReturnSuccessResponse()
    {
        // Arrange
        var command = new LoginCommand
        {
            Email = "test@example.com",
            Password = "Password123!"
        };

        var loginResponse = new LoginResponse
        {
            Token = "valid-token",
            UserId = "user-id",
            Email = "test@example.com",
            UserName = "testuser"
        };

        var authResult = BaseResponse<LoginResponse>.Success(loginResponse, "Login successful");

        _mockAuthService.Setup(x => x.LoginAsync(command.Email, command.Password))
            .ReturnsAsync(authResult);

        // Act
        var result = await _handler.HandleAsync(command);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Token.Should().NotBeNullOrEmpty();
        result.Data.Email.Should().Be(command.Email);
    }

    [Fact]
    public async Task HandleAsync_WithInvalidCredentials_ShouldReturnUnauthorizedResponse()
    {
        // Arrange
        var command = new LoginCommand
        {
            Email = "test@example.com",
            Password = "WrongPassword123!"
        };

        var authResult = BaseResponse<LoginResponse>.Unauthorized("Invalid email or password");

        _mockAuthService.Setup(x => x.LoginAsync(command.Email, command.Password))
            .ReturnsAsync(authResult);

        // Act
        var result = await _handler.HandleAsync(command);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(401);
        result.Message.Should().Contain("Invalid email or password");
    }


} 