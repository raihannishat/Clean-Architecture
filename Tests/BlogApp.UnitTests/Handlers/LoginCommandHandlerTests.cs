namespace BlogApp.UnitTests.Handlers;

public class LoginCommandHandlerTests
{
    private readonly Mock<IAuthService> _mockAuthService;
    private readonly Mock<IOutboxService> _mockOutboxService;
    private readonly LoginCommandHandler _handler;
    
    public LoginCommandHandlerTests()
    {
        _mockAuthService = new Mock<IAuthService>();
        _mockOutboxService = new Mock<IOutboxService>();
        _handler = new LoginCommandHandler(_mockAuthService.Object, _mockOutboxService.Object);
    }

    [Fact]
    public async Task HandleAsync_WithValidCredentials_ShouldReturnSuccessResponse()
    {
        // Arrange
        var command = new LoginCommand("test@example.com", "Password123!");
        var expectedResponse = BaseResponse<LoginResponse>.Success(new LoginResponse { Email = command.Email, UserName = "testuser" }, "Login successful");
        _mockAuthService.Setup(x => x.LoginAsync(command.Email, command.Password)).ReturnsAsync(expectedResponse);
        _mockOutboxService.Setup(x => x.AddAsync(nameof(LoginCommand), command, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        // Act
        var result = await _handler.HandleAsync(command);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        _mockOutboxService.Verify(x => x.AddAsync(nameof(LoginCommand), command, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_WithInvalidCredentials_ShouldReturnUnauthorized()
    {
        // Arrange
        var command = new LoginCommand("wrong@example.com", "wrongpass");
        var expectedResponse = BaseResponse<LoginResponse>.Unauthorized("Invalid credentials");
        _mockAuthService.Setup(x => x.LoginAsync(command.Email, command.Password)).ReturnsAsync(expectedResponse);

        // Act
        var result = await _handler.HandleAsync(command);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(401);
        _mockOutboxService.Verify(x => x.AddAsync(It.IsAny<string>(), It.IsAny<LoginCommand>(), It.IsAny<CancellationToken>()), Times.Never);
    }
} 