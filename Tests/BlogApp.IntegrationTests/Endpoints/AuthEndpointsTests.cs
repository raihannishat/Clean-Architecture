using BlogApp.API.Application.Features.Auth.Commands;
using BlogApp.API.TestHost;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Text;
using System.Text.Json;
using Xunit;

namespace BlogApp.IntegrationTests.Endpoints;

public class AuthEndpointsTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public AuthEndpointsTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task Login_WithValidCredentials_ShouldReturnSuccess()
    {
        // Arrange
        var loginRequest = new LoginCommand
        {
            Email = "test@example.com",
            Password = "Password123!"
        };

        var json = JsonSerializer.Serialize(loginRequest);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/dispatch", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var responseContent = await response.Content.ReadAsStringAsync();
        responseContent.Should().Contain("LoginCommand");
    }

    [Fact]
    public async Task Login_WithInvalidCredentials_ShouldReturnBadRequest()
    {
        // Arrange
        var loginRequest = new LoginCommand
        {
            Email = "",
            Password = ""
        };

        var json = JsonSerializer.Serialize(loginRequest);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/dispatch", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Register_WithValidData_ShouldReturnSuccess()
    {
        // Arrange
        var registerRequest = new RegisterCommand
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com",
            UserName = "johndoe",
            Password = "Password123!",
            ConfirmPassword = "Password123!"
        };

        var json = JsonSerializer.Serialize(registerRequest);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/dispatch", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var responseContent = await response.Content.ReadAsStringAsync();
        responseContent.Should().Contain("RegisterCommand");
    }

    [Fact]
    public async Task Register_WithInvalidData_ShouldReturnBadRequest()
    {
        // Arrange
        var registerRequest = new RegisterCommand
        {
            FirstName = "",
            LastName = "",
            Email = "invalid-email",
            UserName = "",
            Password = "weak",
            ConfirmPassword = "different"
        };

        var json = JsonSerializer.Serialize(registerRequest);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/dispatch", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Dispatch_WithInvalidCommandType_ShouldReturnBadRequest()
    {
        // Arrange
        var invalidRequest = new { InvalidProperty = "test" };
        var json = JsonSerializer.Serialize(invalidRequest);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/dispatch", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Dispatch_WithEmptyBody_ShouldReturnBadRequest()
    {
        // Arrange
        var content = new StringContent("", Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/dispatch", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
} 