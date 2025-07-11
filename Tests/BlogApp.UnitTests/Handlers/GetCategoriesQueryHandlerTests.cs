using BlogApp.API.Application.Features.Blog.Queries;
using BlogApp.API.Application.Common;
using BlogApp.API.Core.Entities;
using BlogApp.API.Infrastructure.Persistence.UnitOfWork.Interfaces;
using BlogApp.API.Infrastructure.Persistence.Repositories.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace BlogApp.UnitTests.Handlers;

public class GetCategoriesQueryHandlerTests
{
    private readonly Mock<IUnitOfWorkFactory> _mockUnitOfWorkFactory;
    private readonly Mock<IQueryUnitOfWork> _mockUnitOfWork;
    private readonly GetCategoriesQueryHandler _handler;

    public GetCategoriesQueryHandlerTests()
    {
        _mockUnitOfWorkFactory = new Mock<IUnitOfWorkFactory>();
        _mockUnitOfWork = new Mock<IQueryUnitOfWork>();
        _mockUnitOfWorkFactory.Setup(x => x.CreateQueryUnitOfWork()).Returns(_mockUnitOfWork.Object);
        _handler = new GetCategoriesQueryHandler(_mockUnitOfWorkFactory.Object);
    }

    [Fact]
    public async Task HandleAsync_WithValidQuery_ShouldReturnSuccessResponse()
    {
        // Arrange
        var query = new GetCategoriesQuery();

        var categories = new List<Category>
        {
            new()
            {
                Id = 1,
                Name = "Technology",
                Slug = "technology",
                IconClass = "fas fa-laptop",
                Color = "#007bff"
            },
            new()
            {
                Id = 2,
                Name = "Travel",
                Slug = "travel",
                IconClass = "fas fa-plane",
                Color = "#28a745"
            },
            new()
            {
                Id = 3,
                Name = "Food",
                Slug = "food",
                IconClass = "fas fa-utensils",
                Color = "#dc3545"
            }
        };

        var mockCategoryRepo = new Mock<IQueryRepository<Category>>();
        mockCategoryRepo.Setup(x => x.GetAllAsync())
            .ReturnsAsync(categories);

        _mockUnitOfWork.Setup(x => x.Repository<Category>()).Returns(mockCategoryRepo.Object);

        // Act
        var result = await _handler.HandleAsync(query);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Count.Should().Be(3);
        result.Data[0].Name.Should().Be("Technology");
        result.Data[1].Name.Should().Be("Travel");
        result.Data[2].Name.Should().Be("Food");
    }

    [Fact]
    public async Task HandleAsync_WithNoCategories_ShouldReturnEmptyList()
    {
        // Arrange
        var query = new GetCategoriesQuery();

        var mockCategoryRepo = new Mock<IQueryRepository<Category>>();
        mockCategoryRepo.Setup(x => x.GetAllAsync())
            .ReturnsAsync(new List<Category>());

        _mockUnitOfWork.Setup(x => x.Repository<Category>()).Returns(mockCategoryRepo.Object);

        // Act
        var result = await _handler.HandleAsync(query);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Count.Should().Be(0);
    }

    [Fact]
    public async Task HandleAsync_WithSingleCategory_ShouldReturnSingleItem()
    {
        // Arrange
        var query = new GetCategoriesQuery();

        var categories = new List<Category>
        {
            new()
            {
                Id = 1,
                Name = "Technology",
                Slug = "technology",
                IconClass = "fas fa-laptop",
                Color = "#007bff"
            }
        };

        var mockCategoryRepo = new Mock<IQueryRepository<Category>>();
        mockCategoryRepo.Setup(x => x.GetAllAsync())
            .ReturnsAsync(categories);

        _mockUnitOfWork.Setup(x => x.Repository<Category>()).Returns(mockCategoryRepo.Object);

        // Act
        var result = await _handler.HandleAsync(query);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Count.Should().Be(1);
        result.Data[0].Name.Should().Be("Technology");
        result.Data[0].Slug.Should().Be("technology");
    }
} 