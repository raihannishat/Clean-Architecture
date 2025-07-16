namespace BlogApp.UnitTests.Handlers;

public class RegisterCommandHandlerTests
{
    private readonly Mock<IAuthService> _mockAuthService;
    private readonly Mock<IOutboxService> _mockOutboxService;
    private readonly RegisterCommandHandler _handler;

    public RegisterCommandHandlerTests()
    {
        _mockAuthService = new Mock<IAuthService>();
        _mockOutboxService = new Mock<IOutboxService>();
        _handler = new RegisterCommandHandler(_mockAuthService.Object, _mockOutboxService.Object);
    }

    [Fact]
    public async Task HandleAsync_WithValidRegistration_ShouldReturnSuccessResponse()
    {
        // Arrange
        var command = new RegisterCommand("John", "Doe", "john@example.com", "johndoe", "Password123!", "Password123!");
        var expectedResponse = BaseResponse<RegisterResponse>.Success(new RegisterResponse { Email = command.Email, UserName = command.UserName }, "Registration successful");
        _mockAuthService.Setup(x => x.RegisterAsync(command.FirstName, command.LastName, command.Email, command.UserName, command.Password, command.ConfirmPassword)).ReturnsAsync(expectedResponse);
        _mockOutboxService.Setup(x => x.AddAsync(nameof(RegisterCommand), command, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        // Act
        var result = await _handler.HandleAsync(command);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        _mockOutboxService.Verify(x => x.AddAsync(nameof(RegisterCommand), command, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_WithInvalidRegistration_ShouldReturnValidationError()
    {
        // Arrange
        var command = new RegisterCommand("John", "Doe", "john@example.com", "johndoe", "Password123!", "Password123!");
        var expectedResponse = BaseResponse<RegisterResponse>.ValidationError(new List<string> { "Email already exists" }, "Validation failed");
        _mockAuthService.Setup(x => x.RegisterAsync(command.FirstName, command.LastName, command.Email, command.UserName, command.Password, command.ConfirmPassword)).ReturnsAsync(expectedResponse);

        // Act
        var result = await _handler.HandleAsync(command);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(400);
        _mockOutboxService.Verify(x => x.AddAsync(It.IsAny<string>(), It.IsAny<RegisterCommand>(), It.IsAny<CancellationToken>()), Times.Never);
    }
} 