namespace BlogApp.UnitTests.Handlers;

public class GetTagsQueryHandlerTests
{
    private readonly Mock<IUnitOfWorkFactory> _mockUnitOfWorkFactory;
    private readonly Mock<IQueryUnitOfWork> _mockUnitOfWork;
    private readonly GetTagsQueryHandler _handler;
    private readonly Mock<IAutoMapper> _mockMapper;

    public GetTagsQueryHandlerTests()
    {
        _mockUnitOfWorkFactory = new Mock<IUnitOfWorkFactory>();
        _mockUnitOfWork = new Mock<IQueryUnitOfWork>();
        _mockMapper = new Mock<IAutoMapper>();
        _mockUnitOfWorkFactory.Setup(x => x.CreateQueryUnitOfWork()).Returns(_mockUnitOfWork.Object);
        _handler = new BlogApp.API.Application.Features.Blog.Queries.GetTagsQueryHandler(_mockUnitOfWorkFactory.Object, _mockMapper.Object);
    }

    [Fact]
    public async Task HandleAsync_WithValidQuery_ShouldReturnSuccessResponse()
    {
        // Arrange
        var query = new BlogApp.API.Application.Features.Blog.Queries.GetTagsQuery();

        var tags = new List<BlogApp.API.Core.Entities.Tag>
        {
            new BlogApp.API.Core.Entities.Tag { Id = 1, Name = "C#", Slug = "csharp", IsActive = true },
            new BlogApp.API.Core.Entities.Tag { Id = 2, Name = "ASP.NET", Slug = "aspnet", IsActive = true },
            new BlogApp.API.Core.Entities.Tag { Id = 3, Name = "Angular", Slug = "angular", IsActive = true }
        };

        var mockTagRepo = new Mock<BlogApp.API.Infrastructure.Persistence.Repositories.Interfaces.IQueryRepository<BlogApp.API.Core.Entities.Tag>>();
        mockTagRepo.Setup(x => x.GetAllAsync())
            .ReturnsAsync(tags);

        _mockUnitOfWork.Setup(x => x.Repository<BlogApp.API.Core.Entities.Tag>()).Returns(mockTagRepo.Object);
        _mockMapper.Setup(x => x.Map<BlogApp.API.Application.Features.Blog.DTOs.TagDTO>(It.IsAny<BlogApp.API.Core.Entities.Tag>()))
            .Returns((BlogApp.API.Core.Entities.Tag t) => new BlogApp.API.Application.Features.Blog.DTOs.TagDTO(t.Id, t.Name, t.IsActive));

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
        var query = new BlogApp.API.Application.Features.Blog.Queries.GetTagsQuery();

        var mockTagRepo = new Mock<BlogApp.API.Infrastructure.Persistence.Repositories.Interfaces.IQueryRepository<BlogApp.API.Core.Entities.Tag>>();
        mockTagRepo.Setup(x => x.GetAllAsync())
            .ReturnsAsync(new List<BlogApp.API.Core.Entities.Tag>());

        _mockUnitOfWork.Setup(x => x.Repository<BlogApp.API.Core.Entities.Tag>()).Returns(mockTagRepo.Object);
        _mockMapper.Setup(x => x.Map<BlogApp.API.Application.Features.Blog.DTOs.TagDTO>(It.IsAny<BlogApp.API.Core.Entities.Tag>()))
            .Returns((BlogApp.API.Core.Entities.Tag t) => new BlogApp.API.Application.Features.Blog.DTOs.TagDTO(t.Id, t.Name, t.IsActive));

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
        var query = new BlogApp.API.Application.Features.Blog.Queries.GetTagsQuery();

        var tags = new List<BlogApp.API.Core.Entities.Tag>
        {
            new BlogApp.API.Core.Entities.Tag { Id = 1, Name = "C#", Slug = "csharp", IsActive = true }
        };

        var mockTagRepo = new Mock<BlogApp.API.Infrastructure.Persistence.Repositories.Interfaces.IQueryRepository<BlogApp.API.Core.Entities.Tag>>();
        mockTagRepo.Setup(x => x.GetAllAsync())
            .ReturnsAsync(tags);

        _mockUnitOfWork.Setup(x => x.Repository<BlogApp.API.Core.Entities.Tag>()).Returns(mockTagRepo.Object);
        _mockMapper.Setup(x => x.Map<BlogApp.API.Application.Features.Blog.DTOs.TagDTO>(It.IsAny<BlogApp.API.Core.Entities.Tag>()))
            .Returns((BlogApp.API.Core.Entities.Tag t) => new BlogApp.API.Application.Features.Blog.DTOs.TagDTO(t.Id, t.Name, t.IsActive));

        // Act
        var result = await _handler.HandleAsync(query);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Count.Should().Be(1);
        result.Data[0].Name.Should().Be("C#");
    }
} 