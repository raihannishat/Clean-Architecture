using BlogApp.API.Application.Features.Blog.Queries;
using BlogApp.API.Application.Common;
using BlogApp.API.Core.Entities;
using BlogApp.API.Infrastructure.Persistence.UnitOfWork.Interfaces;
using BlogApp.API.Infrastructure.Persistence.Repositories.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace BlogApp.UnitTests.Handlers;

public class GetTagsQueryHandlerTests
{
    private readonly Mock<IUnitOfWorkFactory> _mockUnitOfWorkFactory;
    private readonly Mock<IQueryUnitOfWork> _mockUnitOfWork;
    private readonly GetTagsQueryHandler _handler;

    public GetTagsQueryHandlerTests()
    {
        _mockUnitOfWorkFactory = new Mock<IUnitOfWorkFactory>();
        _mockUnitOfWork = new Mock<IQueryUnitOfWork>();
        _mockUnitOfWorkFactory.Setup(x => x.CreateQueryUnitOfWork()).Returns(_mockUnitOfWork.Object);
        _handler = new GetTagsQueryHandler(_mockUnitOfWorkFactory.Object);
    }

    [Fact]
    public async Task HandleAsync_WithValidQuery_ShouldReturnSuccessResponse()
    {
        // Arrange
        var query = new GetTagsQuery();

        var tags = new List<Tag>
        {
            new()
            {
                Id = 1,
                Name = "C#",
                Slug = "csharp",
                IconClass = "fas fa-code",
                Color = "#007bff"
            },
            new()
            {
                Id = 2,
                Name = "ASP.NET",
                Slug = "aspnet",
                IconClass = "fas fa-server",
                Color = "#28a745"
            },
            new()
            {
                Id = 3,
                Name = "Angular",
                Slug = "angular",
                IconClass = "fab fa-angular",
                Color = "#dc3545"
            }
        };

        var mockTagRepo = new Mock<IQueryRepository<Tag>>();
        mockTagRepo.Setup(x => x.GetAllAsync())
            .ReturnsAsync(tags);

        _mockUnitOfWork.Setup(x => x.Repository<Tag>()).Returns(mockTagRepo.Object);

        // Act
        var result = await _handler.HandleAsync(query);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Count.Should().Be(3);
        result.Data[0].Name.Should().Be("C#");
        result.Data[1].Name.Should().Be("ASP.NET");
        result.Data[2].Name.Should().Be("Angular");
    }

    [Fact]
    public async Task HandleAsync_WithNoTags_ShouldReturnEmptyList()
    {
        // Arrange
        var query = new GetTagsQuery();

        var mockTagRepo = new Mock<IQueryRepository<Tag>>();
        mockTagRepo.Setup(x => x.GetAllAsync())
            .ReturnsAsync(new List<Tag>());

        _mockUnitOfWork.Setup(x => x.Repository<Tag>()).Returns(mockTagRepo.Object);

        // Act
        var result = await _handler.HandleAsync(query);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Count.Should().Be(0);
    }

    [Fact]
    public async Task HandleAsync_WithSingleTag_ShouldReturnSingleItem()
    {
        // Arrange
        var query = new GetTagsQuery();

        var tags = new List<Tag>
        {
            new()
            {
                Id = 1,
                Name = "C#",
                Slug = "csharp",
                IconClass = "fas fa-code",
                Color = "#007bff"
            }
        };

        var mockTagRepo = new Mock<IQueryRepository<Tag>>();
        mockTagRepo.Setup(x => x.GetAllAsync())
            .ReturnsAsync(tags);

        _mockUnitOfWork.Setup(x => x.Repository<Tag>()).Returns(mockTagRepo.Object);

        // Act
        var result = await _handler.HandleAsync(query);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Count.Should().Be(1);
        result.Data[0].Name.Should().Be("C#");
        result.Data[0].Slug.Should().Be("csharp");
    }
} 