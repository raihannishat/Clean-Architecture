using BlogApp.API.Application.Features.Blog.Commands;
using BlogApp.API.Application.Features.Blog.Queries;
using BlogApp.API.TestHost;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Text;
using System.Text.Json;
using Xunit;

namespace BlogApp.IntegrationTests.Endpoints;

public class BlogEndpointsTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public BlogEndpointsTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task CreateBlogPost_WithValidData_ShouldReturnSuccess()
    {
        // Arrange
        var createBlogPostRequest = new CreateBlogPostCommand
        {
            Title = "Test Blog Post",
            Content = "This is a test blog post content.",
            Slug = "test-blog-post",
            CategoryId = 1,
            AuthorId = "user-id"
        };

        var json = JsonSerializer.Serialize(createBlogPostRequest);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/dispatch", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var responseContent = await response.Content.ReadAsStringAsync();
        responseContent.Should().Contain("CreateBlogPostCommand");
    }

    [Fact]
    public async Task CreateBlogPost_WithInvalidData_ShouldReturnBadRequest()
    {
        // Arrange
        var createBlogPostRequest = new CreateBlogPostCommand
        {
            Title = "",
            Content = "",
            Slug = "",
            CategoryId = 0,
            AuthorId = ""
        };

        var json = JsonSerializer.Serialize(createBlogPostRequest);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/dispatch", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetBlogPosts_WithValidQuery_ShouldReturnSuccess()
    {
        // Arrange
        var getBlogPostsRequest = new GetBlogPostsQuery
        {
            Page = 1,
            PageSize = 10
        };

        var json = JsonSerializer.Serialize(getBlogPostsRequest);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/dispatch", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var responseContent = await response.Content.ReadAsStringAsync();
        responseContent.Should().Contain("GetBlogPostsQuery");
    }

    [Fact]
    public async Task GetBlogPosts_WithInvalidPagination_ShouldReturnBadRequest()
    {
        // Arrange
        var getBlogPostsRequest = new GetBlogPostsQuery
        {
            Page = 0,
            PageSize = 0
        };

        var json = JsonSerializer.Serialize(getBlogPostsRequest);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/dispatch", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetBlogPostBySlug_WithValidSlug_ShouldReturnSuccess()
    {
        // Arrange
        var getBlogPostBySlugRequest = new GetBlogPostBySlugQuery
        {
            Slug = "test-blog-post"
        };

        var json = JsonSerializer.Serialize(getBlogPostBySlugRequest);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/dispatch", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var responseContent = await response.Content.ReadAsStringAsync();
        responseContent.Should().Contain("GetBlogPostBySlugQuery");
    }

    [Fact]
    public async Task GetBlogPostBySlug_WithEmptySlug_ShouldReturnBadRequest()
    {
        // Arrange
        var getBlogPostBySlugRequest = new GetBlogPostBySlugQuery
        {
            Slug = ""
        };

        var json = JsonSerializer.Serialize(getBlogPostBySlugRequest);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/dispatch", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task SearchPosts_WithValidSearchTerm_ShouldReturnSuccess()
    {
        // Arrange
        var searchPostsRequest = new SearchPostsQuery
        {
            SearchTerm = "technology",
            Page = 1,
            PageSize = 10
        };

        var json = JsonSerializer.Serialize(searchPostsRequest);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/dispatch", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var responseContent = await response.Content.ReadAsStringAsync();
        responseContent.Should().Contain("SearchPostsQuery");
    }

    [Fact]
    public async Task GetCategories_ShouldReturnSuccess()
    {
        // Arrange
        var getCategoriesRequest = new GetCategoriesQuery();

        var json = JsonSerializer.Serialize(getCategoriesRequest);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/dispatch", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var responseContent = await response.Content.ReadAsStringAsync();
        responseContent.Should().Contain("GetCategoriesQuery");
    }

    [Fact]
    public async Task GetTags_ShouldReturnSuccess()
    {
        // Arrange
        var getTagsRequest = new GetTagsQuery();

        var json = JsonSerializer.Serialize(getTagsRequest);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/dispatch", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var responseContent = await response.Content.ReadAsStringAsync();
        responseContent.Should().Contain("GetTagsQuery");
    }
} 