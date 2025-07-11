using BlogApp.API.Api.Endpoints;
using BlogApp.API.Core.Entities;
using BlogApp.API.Infrastructure.Persistence.Contexts;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Text;
using System.Text.Json;

namespace BlogApp.IntegrationTests.Controllers;

public class DispatcherEndpointTests : IClassFixture<WebApplicationFactory<BlogApp.API.TestHost>>
{
    private readonly WebApplicationFactory<BlogApp.API.TestHost> _factory;
    private readonly HttpClient _client;

    public DispatcherEndpointTests(WebApplicationFactory<BlogApp.API.TestHost> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<CommandDbContext>));

                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                services.AddDbContext<CommandDbContext>(options =>
                {
                    options.UseInMemoryDatabase("TestCommandDb");
                });

                var queryDescriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<QueryDbContext>));

                if (queryDescriptor != null)
                {
                    services.Remove(queryDescriptor);
                }

                services.AddDbContext<QueryDbContext>(options =>
                {
                    options.UseInMemoryDatabase("TestQueryDb");
                });
            });
        });

        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task Dispatcher_WithValidLoginCommand_ShouldReturnSuccessResponse()
    {
        // Arrange
        var request = new
        {
            operation = "LoginCommand",
            data = new
            {
                email = "admin@blogapp.com",
                password = "Admin123!"
            }
        };

        var json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/dispatcher", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var responseContent = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<DispatcherResponse>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        result.Should().NotBeNull();
        result!.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
    }

    [Fact]
    public async Task Dispatcher_WithInvalidOperation_ShouldReturnErrorResponse()
    {
        // Arrange
        var request = new
        {
            operation = "NonExistentOperation",
            data = new { }
        };

        var json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/dispatcher", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        
        var responseContent = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<DispatcherResponse>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        result.Should().NotBeNull();
        result!.IsSuccess.Should().BeFalse();
        result.Message.Should().Contain("Operation not found");
    }

    [Fact]
    public async Task Dispatcher_WithInvalidJson_ShouldReturnBadRequest()
    {
        // Arrange
        var invalidJson = "{ invalid json }";
        var content = new StringContent(invalidJson, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/dispatcher", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Dispatcher_WithGetBlogPostsQuery_ShouldReturnSuccessResponse()
    {
        // Arrange
        var request = new
        {
            operation = "GetBlogPostsQuery",
            data = new
            {
                page = 1,
                pageSize = 10
            }
        };

        var json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/dispatcher", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var responseContent = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<DispatcherResponse>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        result.Should().NotBeNull();
        result!.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
    }

    [Fact]
    public async Task Dispatcher_WithGetCategoriesQuery_ShouldReturnSuccessResponse()
    {
        // Arrange
        var request = new
        {
            operation = "GetCategoriesQuery",
            data = new { }
        };

        var json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/dispatcher", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var responseContent = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<DispatcherResponse>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        result.Should().NotBeNull();
        result!.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
    }

    private class DispatcherResponse
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public object? Data { get; set; }
        public List<string> Errors { get; set; } = new();
        public int StatusCode { get; set; }
    }
} 