using BlogApp.API.Application.Features.Blog.Commands;
using BlogApp.API.Core.Entities;
using BlogApp.API.Infrastructure.Persistence.UnitOfWork.Interfaces;
using BlogApp.API.Infrastructure.Persistence.Repositories.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace BlogApp.UnitTests.Handlers;

public class CreateBlogPostCommandHandlerTests
{
    private readonly Mock<IUnitOfWorkFactory> _mockUnitOfWorkFactory;
    private readonly Mock<ICommandUnitOfWork> _mockUnitOfWork;
    private readonly CreateBlogPostCommandHandler _handler;

    public CreateBlogPostCommandHandlerTests()
    {
        _mockUnitOfWorkFactory = new Mock<IUnitOfWorkFactory>();
        _mockUnitOfWork = new Mock<ICommandUnitOfWork>();
        _mockUnitOfWorkFactory.Setup(x => x.CreateCommandUnitOfWork()).Returns(_mockUnitOfWork.Object);
        _handler = new CreateBlogPostCommandHandler(_mockUnitOfWorkFactory.Object);
    }

    [Fact]
    public async Task HandleAsync_WithValidData_ShouldReturnSuccessResponse()
    {
        // Arrange
        var command = new CreateBlogPostCommand
        {
            Title = "Test Blog Post",
            Content = "This is a test blog post content.",
            Slug = "test-blog-post",
            CategoryId = 1,
            TagIds = new List<int> { 1, 2 },
            AuthorId = "user-id"
        };

        var category = new Category
        {
            Id = 1,
            Name = "Technology",
            Slug = "technology"
        };

        var tags = new List<Tag>
        {
            new() { Id = 1, Name = "C#", Slug = "csharp" },
            new() { Id = 2, Name = "ASP.NET", Slug = "aspnet" }
        };

        var mockCategoryRepo = new Mock<ICommandRepository<Category>>();
        var mockTagRepo = new Mock<ICommandRepository<Tag>>();
        var mockBlogPostRepo = new Mock<ICommandRepository<BlogPost>>();
        var mockBlogPostTagRepo = new Mock<ICommandRepository<BlogPostTag>>();

        mockCategoryRepo.Setup(x => x.GetByIdAsync(command.CategoryId))
            .ReturnsAsync(category);

        mockTagRepo.Setup(x => x.GetByIdAsync(It.IsAny<int>()))
            .ReturnsAsync((int id) => tags.FirstOrDefault(t => t.Id == id));

        mockBlogPostRepo.Setup(x => x.AddAsync(It.IsAny<BlogPost>()))
            .Returns(Task.CompletedTask);

        mockBlogPostTagRepo.Setup(x => x.AddAsync(It.IsAny<BlogPostTag>()))
            .Returns(Task.CompletedTask);

        _mockUnitOfWork.Setup(x => x.Repository<Category>()).Returns(mockCategoryRepo.Object);
        _mockUnitOfWork.Setup(x => x.Repository<Tag>()).Returns(mockTagRepo.Object);
        _mockUnitOfWork.Setup(x => x.Repository<BlogPost>()).Returns(mockBlogPostRepo.Object);
        _mockUnitOfWork.Setup(x => x.Repository<BlogPostTag>()).Returns(mockBlogPostTagRepo.Object);
        _mockUnitOfWork.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        var result = await _handler.HandleAsync(command);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Title.Should().Be(command.Title);
        result.Data.Content.Should().Be(command.Content);
        result.Data.CategoryId.Should().Be(command.CategoryId);
    }

    [Fact]
    public async Task HandleAsync_WithNonExistentCategory_ShouldReturnNotFoundResponse()
    {
        // Arrange
        var command = new CreateBlogPostCommand
        {
            Title = "Test Blog Post",
            Content = "This is a test blog post content.",
            Slug = "test-blog-post",
            CategoryId = 999,
            TagIds = new List<int> { 1, 2 },
            AuthorId = "user-id"
        };

        var mockCategoryRepo = new Mock<ICommandRepository<Category>>();
        mockCategoryRepo.Setup(x => x.GetByIdAsync(command.CategoryId))
            .ReturnsAsync((Category?)null);

        _mockUnitOfWork.Setup(x => x.Repository<Category>()).Returns(mockCategoryRepo.Object);

        // Act
        var result = await _handler.HandleAsync(command);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(404);
        result.Message.Should().Contain("Category with ID 999 not found");
    }

    [Fact]
    public async Task HandleAsync_WithNonExistentTag_ShouldReturnNotFoundResponse()
    {
        // Arrange
        var command = new CreateBlogPostCommand
        {
            Title = "Test Blog Post",
            Content = "This is a test blog post content.",
            Slug = "test-blog-post",
            CategoryId = 1,
            TagIds = new List<int> { 1, 999 },
            AuthorId = "user-id"
        };

        var category = new Category
        {
            Id = 1,
            Name = "Technology",
            Slug = "technology"
        };

        var mockCategoryRepo = new Mock<ICommandRepository<Category>>();
        var mockTagRepo = new Mock<ICommandRepository<Tag>>();

        mockCategoryRepo.Setup(x => x.GetByIdAsync(command.CategoryId))
            .ReturnsAsync(category);

        mockTagRepo.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(new Tag { Id = 1, Name = "C#", Slug = "csharp" });

        mockTagRepo.Setup(x => x.GetByIdAsync(999))
            .ReturnsAsync((Tag?)null);

        _mockUnitOfWork.Setup(x => x.Repository<Category>()).Returns(mockCategoryRepo.Object);
        _mockUnitOfWork.Setup(x => x.Repository<Tag>()).Returns(mockTagRepo.Object);

        // Act
        var result = await _handler.HandleAsync(command);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(404);
        result.Message.Should().Contain("Tag with ID 999 not found");
    }


} 