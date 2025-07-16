namespace BlogApp.UnitTests.Handlers;

public class GetCategoriesQueryHandlerTests
{
    private readonly Mock<IUnitOfWorkFactory> _mockUnitOfWorkFactory;
    private readonly Mock<IQueryUnitOfWork> _mockUnitOfWork;
    private readonly GetCategoriesQueryHandler _handler;
    private readonly Mock<IAutoMapper> _mockMapper;

    public GetCategoriesQueryHandlerTests()
    {
        _mockUnitOfWorkFactory = new Mock<IUnitOfWorkFactory>();
        _mockUnitOfWork = new Mock<IQueryUnitOfWork>();
        _mockMapper = new Mock<IAutoMapper>();
        _mockUnitOfWorkFactory.Setup(x => x.CreateQueryUnitOfWork()).Returns(_mockUnitOfWork.Object);
        _handler = new BlogApp.API.Application.Features.Blog.Queries.GetCategoriesQueryHandler(_mockUnitOfWorkFactory.Object, _mockMapper.Object);
    }

    [Fact]
    public async Task HandleAsync_WithValidQuery_ShouldReturnSuccessResponse()
    {
        // Arrange
        var query = new BlogApp.API.Application.Features.Blog.Queries.GetCategoriesQuery();

        var categories = new List<BlogApp.API.Core.Entities.Category>
        {
            new BlogApp.API.Core.Entities.Category
            {
                Id = 1,
                Name = "Technology",
                Slug = "technology",
                IconClass = "fas fa-laptop",
                Color = "#007bff"
            },
            new BlogApp.API.Core.Entities.Category
            {
                Id = 2,
                Name = "Travel",
                Slug = "travel",
                IconClass = "fas fa-plane",
                Color = "#28a745"
            },
            new BlogApp.API.Core.Entities.Category
            {
                Id = 3,
                Name = "Food",
                Slug = "food",
                IconClass = "fas fa-utensils",
                Color = "#dc3545"
            }
        };

        var mockCategoryRepo = new Mock<BlogApp.API.Infrastructure.Persistence.Repositories.Interfaces.IQueryRepository<BlogApp.API.Core.Entities.Category>>();
        mockCategoryRepo.Setup(x => x.GetAllAsync())
            .ReturnsAsync(categories);

        _mockUnitOfWork.Setup(x => x.Repository<BlogApp.API.Core.Entities.Category>()).Returns(mockCategoryRepo.Object);
        _mockMapper.Setup(x => x.Map<BlogApp.API.Application.Features.Blog.DTOs.CategoryDTO>(It.IsAny<BlogApp.API.Core.Entities.Category>()))
            .Returns((BlogApp.API.Core.Entities.Category c) => new BlogApp.API.Application.Features.Blog.DTOs.CategoryDTO(c.Id, c.Name, c.IsActive));

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
        var query = new BlogApp.API.Application.Features.Blog.Queries.GetCategoriesQuery();

        var mockCategoryRepo = new Mock<BlogApp.API.Infrastructure.Persistence.Repositories.Interfaces.IQueryRepository<BlogApp.API.Core.Entities.Category>>();
        mockCategoryRepo.Setup(x => x.GetAllAsync())
            .ReturnsAsync(new List<BlogApp.API.Core.Entities.Category>());

        _mockUnitOfWork.Setup(x => x.Repository<BlogApp.API.Core.Entities.Category>()).Returns(mockCategoryRepo.Object);
        _mockMapper.Setup(x => x.Map<BlogApp.API.Application.Features.Blog.DTOs.CategoryDTO>(It.IsAny<BlogApp.API.Core.Entities.Category>()))
            .Returns((BlogApp.API.Core.Entities.Category c) => new BlogApp.API.Application.Features.Blog.DTOs.CategoryDTO(c.Id, c.Name, c.IsActive));

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
        var query = new BlogApp.API.Application.Features.Blog.Queries.GetCategoriesQuery();

        var categories = new List<BlogApp.API.Core.Entities.Category>
        {
            new BlogApp.API.Core.Entities.Category
            {
                Id = 1,
                Name = "Technology",
                Slug = "technology",
                IconClass = "fas fa-laptop",
                Color = "#007bff"
            }
        };

        var mockCategoryRepo = new Mock<BlogApp.API.Infrastructure.Persistence.Repositories.Interfaces.IQueryRepository<BlogApp.API.Core.Entities.Category>>();
        mockCategoryRepo.Setup(x => x.GetAllAsync())
            .ReturnsAsync(categories);

        _mockUnitOfWork.Setup(x => x.Repository<BlogApp.API.Core.Entities.Category>()).Returns(mockCategoryRepo.Object);
        _mockMapper.Setup(x => x.Map<BlogApp.API.Application.Features.Blog.DTOs.CategoryDTO>(It.IsAny<BlogApp.API.Core.Entities.Category>()))
            .Returns((BlogApp.API.Core.Entities.Category c) => new BlogApp.API.Application.Features.Blog.DTOs.CategoryDTO(c.Id, c.Name, c.IsActive));

        // Act
        var result = await _handler.HandleAsync(query);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Count.Should().Be(1);
        result.Data[0].Name.Should().Be("Technology");
        // Slug property does not exist on CategoryDTO
    }
} 