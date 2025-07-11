using BlogApp.API.Application.CQRS;
using BlogApp.API.Application.Common;
using BlogApp.API.Application.Features.Auth.Commands;
using BlogApp.API.Application.Features.Blog.Queries;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace BlogApp.UnitTests.CQRS;

public class MediatorTests
{
    private readonly IServiceProvider _serviceProvider;
    private readonly Mediator _mediator;

    public MediatorTests()
    {
        var services = new ServiceCollection();
        _serviceProvider = services.BuildServiceProvider();
        _mediator = new Mediator(_serviceProvider);
    }

    [Fact]
    public async Task SendAsync_WithValidCommand_ShouldReturnSuccessResponse()
    {
        // Arrange
        var services = new ServiceCollection();
        var mockHandler = new Mock<ICommandHandler<LoginCommand, BaseResponse<LoginResponse>>>();
        var command = new LoginCommand
        {
            Email = "test@example.com",
            Password = "Password123!"
        };

        var expectedResponse = BaseResponse<LoginResponse>.Success(
            new LoginResponse { Email = "test@example.com", UserName = "testuser" },
            "Login successful");

        mockHandler.Setup(x => x.HandleAsync(command))
            .ReturnsAsync(expectedResponse);

        services.AddScoped<ICommandHandler<LoginCommand, BaseResponse<LoginResponse>>>(_ => mockHandler.Object);
        var serviceProvider = services.BuildServiceProvider();
        var mediator = new Mediator(serviceProvider);

        // Act
        var result = await mediator.SendAsync(command);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Email.Should().Be("test@example.com");
    }

    [Fact]
    public async Task SendAsync_WithValidQuery_ShouldReturnSuccessResponse()
    {
        // Arrange
        var services = new ServiceCollection();
        var mockHandler = new Mock<IQueryHandler<GetBlogPostsQuery, BaseResponse<List<BlogPostDTO>>>>();
        var query = new GetBlogPostsQuery
        {
            Page = 1,
            PageSize = 10
        };

        var expectedResponse = BaseResponse<List<BlogPostDTO>>.Success(
            new List<BlogPostDTO>
            {
                new() { Title = "Test Blog Post", Content = "Test content" }
            },
            "Blog posts retrieved successfully");

        mockHandler.Setup(x => x.HandleAsync(query))
            .ReturnsAsync(expectedResponse);

        services.AddScoped<IQueryHandler<GetBlogPostsQuery, BaseResponse<List<BlogPostDTO>>>>(_ => mockHandler.Object);
        var serviceProvider = services.BuildServiceProvider();
        var mediator = new Mediator(serviceProvider);

        // Act
        var result = await mediator.SendAsync(query);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Count.Should().Be(1);
        result.Data[0].Title.Should().Be("Test Blog Post");
    }

    [Fact]
    public async Task SendAsync_WithHandlerNotFound_ShouldThrowException()
    {
        // Arrange
        var command = new LoginCommand
        {
            Email = "test@example.com",
            Password = "Password123!"
        };

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _mediator.SendAsync(command));
    }

    [Fact]
    public async Task SendAsync_WithNullCommand_ShouldThrowException()
    {
        // Arrange
        LoginCommand? command = null;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _mediator.SendAsync(command!));
    }

    [Fact]
    public async Task SendAsync_WithNullQuery_ShouldThrowException()
    {
        // Arrange
        GetBlogPostsQuery? query = null;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _mediator.SendAsync(query!));
    }

    [Fact]
    public async Task SendAsync_WithCommandHandlerThrowingException_ShouldPropagateException()
    {
        // Arrange
        var services = new ServiceCollection();
        var mockHandler = new Mock<ICommandHandler<LoginCommand, BaseResponse<LoginResponse>>>();
        var command = new LoginCommand
        {
            Email = "test@example.com",
            Password = "Password123!"
        };

        mockHandler.Setup(x => x.HandleAsync(command))
            .ThrowsAsync(new InvalidOperationException("Test exception"));

        services.AddScoped<ICommandHandler<LoginCommand, BaseResponse<LoginResponse>>>(_ => mockHandler.Object);
        var serviceProvider = services.BuildServiceProvider();
        var mediator = new Mediator(serviceProvider);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => mediator.SendAsync(command));
    }

    [Fact]
    public async Task SendAsync_WithQueryHandlerThrowingException_ShouldPropagateException()
    {
        // Arrange
        var services = new ServiceCollection();
        var mockHandler = new Mock<IQueryHandler<GetBlogPostsQuery, BaseResponse<List<BlogPostDTO>>>>();
        var query = new GetBlogPostsQuery
        {
            Page = 1,
            PageSize = 10
        };

        mockHandler.Setup(x => x.HandleAsync(query))
            .ThrowsAsync(new InvalidOperationException("Test exception"));

        services.AddScoped<IQueryHandler<GetBlogPostsQuery, BaseResponse<List<BlogPostDTO>>>>(_ => mockHandler.Object);
        var serviceProvider = services.BuildServiceProvider();
        var mediator = new Mediator(serviceProvider);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => mediator.SendAsync(query));
    }
} 