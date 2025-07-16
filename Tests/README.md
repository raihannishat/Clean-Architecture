# BlogApp Test Suite

This directory contains comprehensive tests for the BlogApp project, organized into three test projects covering different testing levels.

## 📁 Test Projects Structure

```
Tests/
├── BlogApp.UnitTests/           # Unit Tests (xUnit)
│   ├── Handlers/               # Command/Query Handler Tests
│   ├── Validators/             # Validator Tests
│   ├── Services/               # Service Tests
│   ├── Repositories/           # Repository Tests
│   ├── UnitOfWork/             # Unit of Work Tests
│   ├── Factories/              # Factory Tests
│   ├── CQRS/                   # Mediator/CQRS Tests
│   └── BlogApp.UnitTests.csproj
├── BlogApp.IntegrationTests/    # Integration Tests
│   ├── Endpoints/              # API Endpoint Tests
│   ├── Controllers/            # Dispatcher Endpoint Tests
│   └── BlogApp.IntegrationTests.csproj
├── BlogApp.E2ETests/           # End-to-End Tests
│   ├── UI/                     # UI Automation Tests
│   └── BlogApp.E2ETests.csproj
└── README.md                   # This file
```

## 🧪 Test Types

### 1. Unit Tests (BlogApp.UnitTests)
- **Framework**: xUnit
- **Purpose**: Test individual components in isolation
- **Coverage**: Command/Query handlers, validators, services, repositories, unit of work, CQRS
- **Dependencies**: Moq (mocking), FluentAssertions (assertions)

### 2. Integration Tests (BlogApp.IntegrationTests)
- **Framework**: xUnit + WebApplicationFactory
- **Purpose**: Test API endpoints and database integration
- **Coverage**: Dispatcher endpoint, database operations, authentication, BaseResponse structure
- **Dependencies**: In-memory database, WebApplicationFactory

### 3. End-to-End Tests (BlogApp.E2ETests)
- **Framework**: xUnit + Selenium WebDriver
- **Purpose**: Test complete user workflows
- **Coverage**: Angular frontend + API integration, UI interactions, user journeys
- **Dependencies**: Selenium WebDriver, Chrome browser

## 📊 Test Coverage

### Unit Tests Coverage
- **Command Handlers**: Login, Register, CreateBlogPost
- **Query Handlers**: GetBlogPosts, GetBlogPostBySlug, GetCategories, GetTags, SearchPosts, GetComments
- **Validation**: Input validation for all commands/queries (Login, Register, CreateBlogPost, GetBlogPosts, GetBlogPostBySlug, SearchPosts, GetComments)
- **Services**: AuthService, BlogService
- **Repositories**: CommandRepository, QueryRepository
- **Unit of Work**: CommandUnitOfWork, QueryUnitOfWork
- **CQRS**: Mediator pattern, handler dispatching
- **Error Handling**: Exception scenarios, validation errors

### Integration Tests Coverage
- **Dispatcher Endpoint**: All operation types
- **Database Operations**: CRUD operations with in-memory database
- **Authentication**: Login/logout flows
- **Response Format**: BaseResponse structure validation

### E2E Tests Coverage
- **User Authentication**: Login/logout workflows
- **Blog Management**: View posts, create posts
- **Navigation**: Page transitions and routing
- **API Integration**: Frontend-backend communication

## 📚 Related Documentation

For API usage, dispatcher pattern, request/response format, and operation mapping, see the Docs/ directory at the project root:

- `Docs/API_TESTING_GUIDE.md`
- `Docs/DISPATCHER_USAGE_EXAMPLES.md`
- `Docs/GENERIC_RESPONSE_GUIDE.md`
- `Docs/PREFIX_BASED_DISPATCHER_USAGE.md`
- `Docs/BASERESPONSE_MIGRATION_SUMMARY.md`

## 🛠️ Test Configuration

### Unit Tests Configuration
```xml
<!-- BlogApp.UnitTests.csproj -->
<PackageReference Include="xunit" Version="2.6.0" />
<PackageReference Include="Moq" Version="4.20.72" />
<PackageReference Include="FluentAssertions" Version="8.4.0" />
<PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.14.1" />
```

### Integration Tests Configuration
```xml
<!-- BlogApp.IntegrationTests.csproj -->
<PackageReference Include="Microsoft.AspNetCore.Mvc.Testing" Version="9.0.7" />
<PackageReference Include="Microsoft.EntityFrameworkCore.InMemory" Version="9.0.7" />
<PackageReference Include="FluentAssertions" Version="8.4.0" />
```

### E2E Tests Configuration
```xml
<!-- BlogApp.E2ETests.csproj -->
<PackageReference Include="Selenium.WebDriver" Version="4.34.0" />
<PackageReference Include="Selenium.Support" Version="4.34.0" />
<PackageReference Include="FluentAssertions" Version="8.4.0" />
```

## 🔧 Test Utilities

### Test Data Builders
```csharp
public static class TestDataBuilder
{
    public static LoginCommand CreateValidLoginCommand() =>
        new() { Email = "test@example.com", Password = "Password123!" };

    public static CreateBlogPostCommand CreateValidBlogPostCommand() =>
        new() { Title = "Test Post", Content = "Test content", CategoryId = 1 };
}
```

### Mock Helpers
```csharp
public static class MockHelper
{
    public static Mock<UserManager<ApplicationUser>> CreateMockUserManager()
    {
        var store = new Mock<IUserStore<ApplicationUser>>();
        return new Mock<UserManager<ApplicationUser>>(store.Object, null, null, null, null, null, null, null, null);
    }
}
```

## 📈 Best Practices

### Unit Tests
1. **Arrange-Act-Assert**: Follow the AAA pattern
2. **Descriptive Names**: Use clear, descriptive test method names
3. **Single Responsibility**: Each test should test one thing
4. **Mock External Dependencies**: Don't test external systems
5. **Test Edge Cases**: Include boundary conditions and error scenarios

### Integration Tests
1. **Use In-Memory Database**: Fast execution, isolated tests
2. **Test Complete Flows**: Test end-to-end API operations
3. **Validate Response Format**: Ensure consistent response structure
4. **Clean Up**: Reset database state between tests

### E2E Tests
1. **Test User Journeys**: Focus on complete user workflows
2. **Use Explicit Waits**: Wait for elements to be present/visible
3. **Headless Mode**: Run in CI/CD environments
4. **Test Real Scenarios**: Test actual user interactions

## 🚨 Troubleshooting

### Common Issues

1. **E2E Tests Failing**
   - Ensure Chrome browser is installed
   - Check if API and Angular apps are running
   - Verify URLs in test configuration

2. **Integration Tests Failing**
   - Check in-memory database configuration
   - Verify service registration in test setup
   - Ensure proper test isolation

3. **Unit Tests Failing**
   - Check mock setup and expectations
   - Verify test data and assertions
   - Ensure proper dependency injection

### Debug Mode
```bash
# Run tests in debug mode
dotnet test --logger "console;verbosity=detailed" --filter "FullyQualifiedName~TestName"
```

## 📝 Adding New Tests

### Adding Unit Tests
1. Create test class in appropriate namespace
2. Follow naming convention: `{ClassName}Tests`
3. Use descriptive test method names
4. Mock external dependencies
5. Test both success and failure scenarios

### Adding Integration Tests
1. Create test class inheriting from `IClassFixture<WebApplicationFactory<Program>>`
2. Configure in-memory database in constructor
3. Test complete API request/response cycles
4. Validate response structure and content

### Adding E2E Tests
1. Create test class implementing `IDisposable`
2. Set up WebDriver in constructor
3. Test complete user workflows
4. Use explicit waits for UI elements
5. Clean up resources in Dispose method

## 🎯 Test Metrics

- **Unit Test Coverage**: Target >80%
- **Integration Test Coverage**: Target >70%
- **E2E Test Coverage**: Target >50%
- **Test Execution Time**: <5 minutes for full suite

## 📚 Additional Resources

- [xUnit Documentation](https://xunit.net/)
- [Moq Documentation](https://github.com/moq/moq4)
- [FluentAssertions Documentation](https://fluentassertions.com/)
- [Selenium WebDriver Documentation](https://www.selenium.dev/)
- [ASP.NET Core Testing](https://docs.microsoft.com/en-us/aspnet/core/test/) 