using BlogApp.API.Infrastructure.Persistence.UnitOfWork.Implementations;
using BlogApp.API.Infrastructure.Persistence.UnitOfWork.Interfaces;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace BlogApp.UnitTests.Factories;

public class UnitOfWorkFactoryTests
{
    private readonly Mock<IConfiguration> _mockConfiguration;
    private readonly UnitOfWorkFactory _factory;

    public UnitOfWorkFactoryTests()
    {
        _mockConfiguration = new Mock<IConfiguration>();
        _factory = new UnitOfWorkFactory(_mockConfiguration.Object);
    }

    [Fact]
    public void CreateCommandUnitOfWork_ShouldReturnCommandUnitOfWork()
    {
        // Arrange
        SetupConfigurationForCommandDb();

        // Act
        var result = _factory.CreateCommandUnitOfWork();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<CommandUnitOfWork>();
    }

    [Fact]
    public void CreateQueryUnitOfWork_ShouldReturnQueryUnitOfWork()
    {
        // Arrange
        SetupConfigurationForQueryDb();

        // Act
        var result = _factory.CreateQueryUnitOfWork();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<QueryUnitOfWork>();
    }

    [Fact]
    public void CreateCommandUnitOfWork_WithMultipleCalls_ShouldReturnDifferentInstances()
    {
        // Arrange
        SetupConfigurationForCommandDb();

        // Act
        var unitOfWork1 = _factory.CreateCommandUnitOfWork();
        var unitOfWork2 = _factory.CreateCommandUnitOfWork();

        // Assert
        unitOfWork1.Should().NotBeSameAs(unitOfWork2);
    }

    [Fact]
    public void CreateQueryUnitOfWork_WithMultipleCalls_ShouldReturnDifferentInstances()
    {
        // Arrange
        SetupConfigurationForQueryDb();

        // Act
        var unitOfWork1 = _factory.CreateQueryUnitOfWork();
        var unitOfWork2 = _factory.CreateQueryUnitOfWork();

        // Assert
        unitOfWork1.Should().NotBeSameAs(unitOfWork2);
    }

    [Fact]
    public void CreateCommandUnitOfWork_ShouldCreateValidContext()
    {
        // Arrange
        SetupConfigurationForCommandDb();

        // Act
        var unitOfWork = _factory.CreateCommandUnitOfWork();

        // Assert
        unitOfWork.Should().NotBeNull();
        unitOfWork.GetType().Name.Should().Be("CommandUnitOfWork");
    }

    [Fact]
    public void CreateQueryUnitOfWork_ShouldCreateValidContext()
    {
        // Arrange
        SetupConfigurationForQueryDb();

        // Act
        var unitOfWork = _factory.CreateQueryUnitOfWork();

        // Assert
        unitOfWork.Should().NotBeNull();
        unitOfWork.GetType().Name.Should().Be("QueryUnitOfWork");
    }

    private void SetupConfigurationForCommandDb()
    {
        var commandConnectionString = "Host=localhost;Database=BlogApp_Commands;Username=postgres;Password=password";
        _mockConfiguration.Setup(x => x.GetConnectionString("CommandConnection"))
            .Returns(commandConnectionString);
    }

    private void SetupConfigurationForQueryDb()
    {
        var queryConnectionString = "mongodb://localhost:27017/BlogApp_Queries";
        _mockConfiguration.Setup(x => x.GetConnectionString("QueryConnection"))
            .Returns(queryConnectionString);
    }
} 