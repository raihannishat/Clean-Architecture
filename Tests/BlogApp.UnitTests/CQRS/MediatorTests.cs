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
        var command = new LoginCommand("test@example.com", "Password123!");

        var expectedResponse = BaseResponse<LoginResponse>.Success(
            new LoginResponse { Email = "test@example.com", UserName = "testuser" },
            "Login successful");

        mockHandler.Setup(x => x.HandleAsync(It.IsAny<LoginCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        services.AddScoped<ICommandHandler<LoginCommand, BaseResponse<LoginResponse>>>(_ => mockHandler.Object);
        var serviceProvider = services.BuildServiceProvider();
        var mediator = new Mediator(serviceProvider);

        // Act
        var result = await mediator.SendAsync<LoginCommand, BaseResponse<LoginResponse>>(command, CancellationToken.None);

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
        var query = new GetBlogPostsQuery(1, 10);

        var blogPostDto = new BlogPostDTO(
            1,
            "Test Blog Post",
            "Test content",
            "test-blog-post",
            "author",
            "category",
            System.Array.Empty<string>()
        );
        var blogPosts = new List<BlogPostDTO> { blogPostDto };
        var expectedResponse = BaseResponse<List<BlogPostDTO>>.Success(
            blogPosts,
            "Blog posts retrieved successfully"
        );

        mockHandler.Setup(x => x.HandleAsync(It.IsAny<GetBlogPostsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        services.AddScoped<IQueryHandler<GetBlogPostsQuery, BaseResponse<List<BlogPostDTO>>>>(_ => mockHandler.Object);
        var serviceProvider = services.BuildServiceProvider();
        var mediator = new Mediator(serviceProvider);

        // Act
        var result = await mediator.SendAsync<GetBlogPostsQuery, BaseResponse<List<BlogPostDTO>>>(query, CancellationToken.None);

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
        var command = new LoginCommand("test@example.com", "Password123!");

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _mediator.SendAsync<LoginCommand, BaseResponse<LoginResponse>>(command));
    }

    [Fact]
    public async Task SendAsync_WithNullCommand_ShouldThrowException()
    {
        // Arrange
        LoginCommand? command = null;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _mediator.SendAsync<LoginCommand, BaseResponse<LoginResponse>>(command!));
    }

    [Fact]
    public async Task SendAsync_WithNullQuery_ShouldThrowException()
    {
        // Arrange
        GetBlogPostsQuery? query = null;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _mediator.SendAsync<GetBlogPostsQuery, BaseResponse<List<BlogPostDTO>>>(query!));
    }

    [Fact]
    public async Task SendAsync_WithCommandHandlerThrowingException_ShouldPropagateException()
    {
        // Arrange
        var services = new ServiceCollection();
        var mockHandler = new Mock<ICommandHandler<LoginCommand, BaseResponse<LoginResponse>>>();
        var command = new LoginCommand("test@example.com", "Password123!");

        mockHandler.Setup(x => x.HandleAsync(It.IsAny<LoginCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Test exception"));

        services.AddScoped<ICommandHandler<LoginCommand, BaseResponse<LoginResponse>>>(_ => mockHandler.Object);
        var serviceProvider = services.BuildServiceProvider();
        var mediator = new Mediator(serviceProvider);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => mediator.SendAsync<LoginCommand, BaseResponse<LoginResponse>>(command));
    }

    [Fact]
    public async Task SendAsync_WithQueryHandlerThrowingException_ShouldPropagateException()
    {
        // Arrange
        var services = new ServiceCollection();
        var mockHandler = new Mock<IQueryHandler<GetBlogPostsQuery, BaseResponse<List<BlogPostDTO>>>>();
        var query = new GetBlogPostsQuery(1, 10);

        mockHandler.Setup(x => x.HandleAsync(It.IsAny<GetBlogPostsQuery>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Test exception"));

        services.AddScoped<IQueryHandler<GetBlogPostsQuery, BaseResponse<List<BlogPostDTO>>>>(_ => mockHandler.Object);
        var serviceProvider = services.BuildServiceProvider();
        var mediator = new Mediator(serviceProvider);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => mediator.SendAsync<GetBlogPostsQuery, BaseResponse<List<BlogPostDTO>>>(query));
    }
} 