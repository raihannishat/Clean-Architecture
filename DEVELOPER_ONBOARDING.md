# Developer Onboarding Guide - BlogSite API

Welcome to the BlogSite API project! This guide will help you get up to speed quickly with our codebase, development practices, and project structure.

## 🎯 Quick Start Checklist

### Prerequisites Setup
- [ ] Install .NET 8.0 SDK or later
- [ ] Install your preferred IDE (Visual Studio, VS Code, or JetBrains Rider)
- [ ] Clone the repository
- [ ] Verify you can build and run the project

### First Day Tasks
- [ ] Read this entire onboarding guide
- [ ] Review the [README.md](README.md) for project overview
- [ ] Explore the [API_SUMMARY.md](API_SUMMARY.md) for endpoint details
- [ ] Study the [CQRS_IMPLEMENTATION.md](CQRS_IMPLEMENTATION.md) for architectural patterns
- [ ] Run the project locally and explore the Swagger UI
- [ ] Make your first small contribution (see [First Contribution](#first-contribution) section)

## 🏗️ Understanding the Architecture

### Clean Architecture Layers

Our project follows **Clean Architecture** principles with these layers:

```
🎯 Domain Layer (BlogSite.Domain)
├── Core business entities (Author, BlogPost, Category, Comment)
├── Domain logic and business rules
└── No dependencies on other layers

🔧 Application Layer (BlogSite.Application)
├── CQRS Commands and Queries
├── Business logic orchestration
├── DTOs and mapping profiles
├── Service interfaces
└── Depends only on Domain layer

🗄️ Infrastructure Layer (BlogSite.Infrastructure)
├── Data access implementation
├── Repository implementations
├── Entity Framework configuration
└── Depends on Domain and Application layers

🌐 Presentation Layer (BlogSite.API)
├── Minimal API endpoints
├── Dependency injection configuration
├── Middleware configuration
└── Depends on all other layers
```

### Key Design Patterns

#### 1. CQRS with MediatR
- **Commands**: Write operations (Create, Update, Delete)
- **Queries**: Read operations (Get, Search, List)
- **Benefits**: Clear separation of concerns, better testability, independent scaling

#### 2. Repository Pattern
- Abstracts data access logic
- Enables easy testing with mock implementations
- Centralizes data access concerns

#### 3. Dependency Injection
- Loose coupling between components
- Easy to test and maintain
- Configuration in `Program.cs`

## 🛠️ Development Workflow

### Setting Up Your Development Environment

1. **Clone and Build**
   ```bash
   git clone <repository-url>
   cd BlogSite
   dotnet restore
   dotnet build
   ```

2. **Run the Application**
   ```bash
   cd src/BlogSite.API
   dotnet run
   ```

3. **Access the Application**
   - API: `http://localhost:5165`
   - Swagger UI: `http://localhost:5165` (served at root)

### Project Structure Deep Dive

```
BlogSite/
├── 📄 BlogSite.sln                    # Solution file
├── 📁 src/
│   ├── 📁 BlogSite.Domain/            # 🎯 Domain Layer
│   │   └── 📁 Entities/               # Core business entities
│   ├── 📁 BlogSite.Application/       # 🔧 Application Layer
│   │   ├── 📁 Commands/               # CQRS write operations
│   │   │   ├── 📁 Authors/            # Author-related commands
│   │   │   ├── 📁 BlogPosts/          # BlogPost-related commands
│   │   │   └── 📁 Categories/         # Category-related commands
│   │   ├── 📁 Queries/                # CQRS read operations
│   │   │   ├── 📁 Authors/            # Author-related queries
│   │   │   ├── 📁 BlogPosts/          # BlogPost-related queries
│   │   │   └── 📁 Categories/         # Category-related queries
│   │   ├── 📁 DTOs/                   # Data Transfer Objects
│   │   ├── 📁 Interfaces/             # Service contracts
│   │   ├── 📁 Services/               # Business logic services
│   │   └── 📁 Mappings/               # AutoMapper profiles
│   ├── 📁 BlogSite.Infrastructure/    # 🗄️ Infrastructure Layer
│   │   ├── 📁 Data/                   # EF Core context and configurations
│   │   └── 📁 Repositories/           # Repository implementations
│   └── 📁 BlogSite.API/              # 🌐 Presentation Layer
│       ├── 📁 Endpoints/              # Minimal API endpoint definitions
│       ├── 📄 Program.cs              # Application entry point and DI setup
│       └── 📄 appsettings.json        # Configuration settings
```

## 🧪 Testing Strategy

### Current Testing Approach
- Manual testing via Swagger UI
- Database seeding for consistent test data
- Comprehensive error handling validation

### Recommended Testing Additions
1. **Unit Tests**: Test individual components in isolation
2. **Integration Tests**: Test API endpoints end-to-end
3. **Repository Tests**: Test data access layer
4. **Command/Query Handler Tests**: Test CQRS implementations

### Testing Best Practices
- Use the AAA pattern (Arrange, Act, Assert)
- Mock external dependencies
- Test both happy path and error scenarios
- Maintain test data independence

## 🔄 Making Changes

### Adding New Features

#### 1. Adding a New Entity
1. Create entity in `BlogSite.Domain/Entities/`
2. Add to DbContext in `BlogSite.Infrastructure/Data/`
3. Create repository interface and implementation
4. Add AutoMapper profile for DTOs
5. Create CQRS commands and queries
6. Add API endpoints

#### 2. Adding New Endpoints
1. Create command/query in appropriate folder
2. Implement handler with business logic
3. Add endpoint in `BlogSite.API/Endpoints/`
4. Update API documentation

#### 3. Adding New Business Rules
1. Implement validation in command handlers
2. Update domain entities if needed
3. Add appropriate error handling
4. Update tests and documentation

### Code Style Guidelines

#### Naming Conventions
- **Classes**: PascalCase (`AuthorService`)
- **Methods**: PascalCase (`GetAuthorByIdAsync`)
- **Properties**: PascalCase (`FirstName`)
- **Parameters**: camelCase (`firstName`)
- **Fields**: camelCase with underscore prefix (`_authorRepository`)

#### CQRS Conventions
- **Commands**: End with "Command" (`CreateAuthorCommand`)
- **Queries**: End with "Query" (`GetAllAuthorsQuery`)
- **Handlers**: End with "Handler" (`CreateAuthorCommandHandler`)
- **DTOs**: End with "Dto" (`AuthorDto`)

#### File Organization
- One class per file
- File name matches class name
- Group related functionality in folders
- Keep using statements organized

## 🎓 Learning Resources

### Understanding Clean Architecture
- Read about [Clean Architecture principles](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- Understand dependency inversion principle
- Learn about separation of concerns

### CQRS and MediatR
- [CQRS Pattern](https://docs.microsoft.com/en-us/azure/architecture/patterns/cqrs)
- [MediatR Documentation](https://github.com/jbogard/MediatR)
- Understanding command vs query responsibilities

### ASP.NET Core
- [Minimal APIs](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis)
- [Dependency Injection](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection)
- [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/)

## 🚀 First Contribution

Here are some good first tasks to get familiar with the codebase:

### Beginner Tasks
1. **Add a new property to an existing entity**
   - Add `PhoneNumber` to Author entity
   - Update DTOs and mappings
   - Test via Swagger UI

2. **Create a new query**
   - `GetAuthorsByBioContainsQuery`
   - Implement handler and endpoint
   - Add to AuthorsController

3. **Improve validation**
   - Add email format validation
   - Add required field validation
   - Improve error messages

### Intermediate Tasks
1. **Add bulk operations**
   - `CreateMultipleAuthorsCommand`
   - `DeleteMultipleAuthorsCommand`

2. **Add search functionality**
   - Search blog posts by title/content
   - Search authors by name
   - Add pagination support

3. **Improve error handling**
   - Create custom exception types
   - Add global exception handling middleware
   - Improve error response format

## 🔧 Common Development Tasks

### Database Changes
1. **Adding new properties**:
   ```bash
   dotnet ef migrations add AddNewProperty
   dotnet ef database update
   ```

2. **Viewing current migration status**:
   ```bash
   dotnet ef migrations list
   ```

### Debugging
- Use breakpoints in handlers for business logic debugging
- Check Entity Framework SQL logging in console
- Use Swagger UI for API testing
- Examine `BlogSite.db` with SQLite browser

### Performance Considerations
- Use async/await properly for I/O operations
- Consider caching for frequently accessed data
- Monitor query performance with EF Core logging
- Use appropriate HTTP status codes

## 🆘 Getting Help

### Internal Resources
- Review existing documentation in the repository
- Check similar implementations in the codebase
- Examine tests for usage examples

### External Resources
- Stack Overflow for specific technical questions
- Microsoft documentation for .NET/EF Core
- GitHub issues for library-specific problems

### Team Communication
- Ask questions in team chat/meetings
- Request code reviews for learning
- Pair program on complex features

## 🎯 Next Steps

After completing your onboarding:

1. **Choose your first task** from the [First Contribution](#first-contribution) section
2. **Set up your development workflow** with your preferred tools
3. **Get familiar with the existing codebase** by exploring different layers
4. **Start contributing** with small, well-tested changes
5. **Gradually take on more complex features** as you become comfortable

## 📚 Additional Documentation

Don't forget to review these project-specific documents:
- [README.md](README.md) - Project overview and setup
- [API_SUMMARY.md](API_SUMMARY.md) - Complete API documentation
- [CQRS_IMPLEMENTATION.md](CQRS_IMPLEMENTATION.md) - Architectural patterns
- [COMPREHENSIVE_API_DOCUMENTATION.md](COMPREHENSIVE_API_DOCUMENTATION.md) - Detailed API reference

Welcome to the team! 🎉

---

**Questions?** Don't hesitate to ask - everyone is here to help you succeed!