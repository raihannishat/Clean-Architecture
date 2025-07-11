using BlogApp.API.Application.Common;
using BlogApp.API.Application.Features.Auth.Commands;
using BlogApp.API.Core.Entities;
using BlogApp.API.Core.Interfaces;
using BlogApp.API.Infrastructure.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;
using Xunit;

namespace BlogApp.UnitTests.Services;

public class AuthServiceTests
{
    private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;
    private readonly Mock<SignInManager<ApplicationUser>> _mockSignInManager;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        var userStore = new Mock<IUserStore<ApplicationUser>>();
        _mockUserManager = new Mock<UserManager<ApplicationUser>>(
            userStore.Object, null!, null!, null!, null!, null!, null!, null!, null!);

        var contextAccessor = new Mock<Microsoft.AspNetCore.Http.IHttpContextAccessor>();
        var userPrincipalFactory = new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>();
        _mockSignInManager = new Mock<SignInManager<ApplicationUser>>(
            _mockUserManager.Object, contextAccessor.Object, userPrincipalFactory.Object, null!, null!, null!, null!);

        var mockConfiguration = new Mock<IConfiguration>();
        _authService = new AuthService(_mockUserManager.Object, mockConfiguration.Object);
    }

    [Fact]
    public async Task LoginAsync_WithValidCredentials_ShouldReturnSuccessResponse()
    {
        // Arrange
        var email = "test@example.com";
        var password = "Password123!";
        var user = new ApplicationUser
        {
            Id = "user-id",
            Email = email,
            UserName = "testuser"
        };

        _mockUserManager.Setup(x => x.FindByEmailAsync(email))
            .ReturnsAsync(user);

        _mockUserManager.Setup(x => x.CheckPasswordAsync(user, password))
            .ReturnsAsync(true);

        _mockUserManager.Setup(x => x.GetRolesAsync(user))
            .ReturnsAsync(new List<string> { "User" });

        _mockSignInManager.Setup(x => x.SignInAsync(user, false, null))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _authService.LoginAsync(email, password);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Email.Should().Be(email);
        result.Data.UserName.Should().Be("testuser");
    }

    [Fact]
    public async Task LoginAsync_WithInvalidEmail_ShouldReturnFailureResponse()
    {
        // Arrange
        var email = "nonexistent@example.com";
        var password = "Password123!";

        _mockUserManager.Setup(x => x.FindByEmailAsync(email))
            .ReturnsAsync((ApplicationUser?)null);

        // Act
        var result = await _authService.LoginAsync(email, password);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(401);
        result.Message.Should().Contain("Invalid credentials");
    }

    [Fact]
    public async Task LoginAsync_WithInvalidPassword_ShouldReturnFailureResponse()
    {
        // Arrange
        var email = "test@example.com";
        var password = "WrongPassword123!";
        var user = new ApplicationUser
        {
            Id = "user-id",
            Email = email,
            UserName = "testuser"
        };

        _mockUserManager.Setup(x => x.FindByEmailAsync(email))
            .ReturnsAsync(user);

        _mockUserManager.Setup(x => x.CheckPasswordAsync(user, password))
            .ReturnsAsync(false);

        // Act
        var result = await _authService.LoginAsync(email, password);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(401);
        result.Message.Should().Contain("Invalid credentials");
    }

    [Fact]
    public async Task RegisterAsync_WithValidData_ShouldReturnSuccessResponse()
    {
        // Arrange
        var firstName = "John";
        var lastName = "Doe";
        var email = "john.doe@example.com";
        var userName = "johndoe";
        var password = "Password123!";
        var confirmPassword = "Password123!";

        var user = new ApplicationUser
        {
            Id = "user-id",
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            UserName = userName
        };

        _mockUserManager.Setup(x => x.FindByEmailAsync(email))
            .ReturnsAsync((ApplicationUser?)null);

        _mockUserManager.Setup(x => x.FindByNameAsync(userName))
            .ReturnsAsync((ApplicationUser?)null);

        _mockUserManager.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), password))
            .ReturnsAsync(IdentityResult.Success);

        _mockUserManager.Setup(x => x.AddToRoleAsync(It.IsAny<ApplicationUser>(), "User"))
            .ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await _authService.RegisterAsync(firstName, lastName, email, userName, password, confirmPassword);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Email.Should().Be(email);
        result.Data.UserName.Should().Be(userName);
    }

    [Fact]
    public async Task RegisterAsync_WithExistingEmail_ShouldReturnFailureResponse()
    {
        // Arrange
        var firstName = "John";
        var lastName = "Doe";
        var email = "existing@example.com";
        var userName = "johndoe";
        var password = "Password123!";
        var confirmPassword = "Password123!";

        var existingUser = new ApplicationUser
        {
            Id = "existing-user-id",
            Email = email
        };

        _mockUserManager.Setup(x => x.FindByEmailAsync(email))
            .ReturnsAsync(existingUser);

        // Act
        var result = await _authService.RegisterAsync(firstName, lastName, email, userName, password, confirmPassword);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(400);
        result.Message.Should().Contain("Email already exists");
    }

    [Fact]
    public async Task RegisterAsync_WithExistingUserName_ShouldReturnFailureResponse()
    {
        // Arrange
        var firstName = "John";
        var lastName = "Doe";
        var email = "new@example.com";
        var userName = "existinguser";
        var password = "Password123!";
        var confirmPassword = "Password123!";

        var existingUser = new ApplicationUser
        {
            Id = "existing-user-id",
            UserName = userName
        };

        _mockUserManager.Setup(x => x.FindByEmailAsync(email))
            .ReturnsAsync((ApplicationUser?)null);

        _mockUserManager.Setup(x => x.FindByNameAsync(userName))
            .ReturnsAsync(existingUser);

        // Act
        var result = await _authService.RegisterAsync(firstName, lastName, email, userName, password, confirmPassword);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(400);
        result.Message.Should().Contain("Username already exists");
    }

    [Fact]
    public async Task RegisterAsync_WithMismatchedPasswords_ShouldReturnFailureResponse()
    {
        // Arrange
        var firstName = "John";
        var lastName = "Doe";
        var email = "new@example.com";
        var userName = "johndoe";
        var password = "Password123!";
        var confirmPassword = "DifferentPassword123!";

        _mockUserManager.Setup(x => x.FindByEmailAsync(email))
            .ReturnsAsync((ApplicationUser?)null);

        _mockUserManager.Setup(x => x.FindByNameAsync(userName))
            .ReturnsAsync((ApplicationUser?)null);

        // Act
        var result = await _authService.RegisterAsync(firstName, lastName, email, userName, password, confirmPassword);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(400);
        result.Message.Should().Contain("Passwords do not match");
    }

    [Fact]
    public async Task RegisterAsync_WithUserCreationFailure_ShouldReturnFailureResponse()
    {
        // Arrange
        var firstName = "John";
        var lastName = "Doe";
        var email = "new@example.com";
        var userName = "johndoe";
        var password = "Password123!";
        var confirmPassword = "Password123!";

        _mockUserManager.Setup(x => x.FindByEmailAsync(email))
            .ReturnsAsync((ApplicationUser?)null);

        _mockUserManager.Setup(x => x.FindByNameAsync(userName))
            .ReturnsAsync((ApplicationUser?)null);

        _mockUserManager.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), password))
            .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "User creation failed" }));

        // Act
        var result = await _authService.RegisterAsync(firstName, lastName, email, userName, password, confirmPassword);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(400);
        result.Message.Should().Contain("User creation failed");
    }
} 