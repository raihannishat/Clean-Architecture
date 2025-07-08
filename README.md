# BlogSite API - Clean Architecture with CQRS

A comprehensive blog management API built with ASP.NET Core 8.0, demonstrating Clean Architecture principles and CQRS (Command Query Responsibility Segregation) patterns using MediatR.

## 🚀 Features

- **Complete Blog Management**: Authors, Categories, Blog Posts, and Comments
- **Publishing Workflow**: Publish/unpublish blog posts
- **Comment Moderation**: Approve/reject comments before publication
- **Clean Architecture**: Proper separation of concerns across layers
- **CQRS Pattern**: Commands and Queries segregation for better scalability
- **RESTful API**: Comprehensive endpoints with Swagger documentation
- **Data Validation**: Robust validation and error handling
- **AutoMapper Integration**: Efficient object-to-object mapping

## 🏗️ Architecture

This project follows **Clean Architecture** principles with clear separation of responsibilities:

### Layers

```
📁 src/
├── 📁 BlogSite.Domain/          # 🎯 Domain Layer
│   └── Core business entities and domain logic
├── 📁 BlogSite.Application/     # 🔧 Application Layer  
│   └── CQRS Commands/Queries, DTOs, Services
├── 📁 BlogSite.Infrastructure/  # 🗄️ Infrastructure Layer
│   └── Data access, repositories, Entity Framework
└── 📁 BlogSite.API/            # 🌐 Presentation Layer
    └── Controllers, configuration, dependency injection
```

### Design Patterns

- **🔄 CQRS Pattern**: Commands for writes, Queries for reads using MediatR
- **📚 Repository Pattern**: Data access abstraction
- **🔧 Dependency Inversion**: Loose coupling between layers
- **🗂️ Service Layer**: Business logic encapsulation
- **🔀 Mediator Pattern**: Decoupled request/response handling

## 🛠️ Technology Stack

### Core Framework
- **ASP.NET Core 8.0** - Web API framework
- **Entity Framework Core 9.0.6** - ORM with SQLite provider
- **MediatR 12.2.0** - CQRS implementation
- **AutoMapper 15.0.0** - Object-to-object mapping

### Database
- **SQLite** - Development database (easily portable)
- **Entity Framework Migrations** - Database versioning

### Development Tools
- **Swagger/OpenAPI** - API documentation
- **CORS** - Cross-origin requests support
- **Dependency Injection** - Built-in IoC container

## 🚀 Getting Started

### Prerequisites
- .NET 8.0 SDK or later
- Your favorite IDE (Visual Studio, VS Code, Rider)

### Installation

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd BlogSite
   ```

2. **Restore dependencies**
   ```bash
   dotnet restore
   ```

3. **Build the solution**
   ```bash
   dotnet build
   ```

4. **Run the API**
   ```bash
   cd src/BlogSite.API
   dotnet run
   ```

5. **Access the API**
   - 🌐 **API Base URL**: `http://localhost:5165`
   - 📖 **Swagger Documentation**: `http://localhost:5165` (served at root)

The database will be automatically created and seeded with sample data on first run.

## 📋 API Endpoints

### 👤 Authors (`/api/authors`)
- `GET /api/authors` - Get all authors
- `GET /api/authors/{id}` - Get author by ID
- `GET /api/authors/email/{email}` - Get author by email
- `POST /api/authors` - Create new author
- `PUT /api/authors/{id}` - Update author
- `DELETE /api/authors/{id}` - Delete author

### 🏷️ Categories (`/api/categories`)
- `GET /api/categories` - Get all categories
- `GET /api/categories/{id}` - Get category by ID
- `GET /api/categories/name/{name}` - Get category by name
- `POST /api/categories` - Create new category
- `PUT /api/categories/{id}` - Update category
- `DELETE /api/categories/{id}` - Delete category

### 📝 Blog Posts (`/api/blogposts`)
- `GET /api/blogposts` - Get all blog posts
- `GET /api/blogposts/published` - Get published posts only
- `GET /api/blogposts/{id}` - Get post by ID
- `GET /api/blogposts/author/{authorId}` - Get posts by author
- `GET /api/blogposts/category/{categoryId}` - Get posts by category
- `POST /api/blogposts` - Create new post
- `PUT /api/blogposts/{id}` - Update post
- `DELETE /api/blogposts/{id}` - Delete post
- `POST /api/blogposts/{id}/publish` - Publish post
- `POST /api/blogposts/{id}/unpublish` - Unpublish post

### 💬 Comments (`/api/comments`)
- `GET /api/comments` - Get all comments
- `GET /api/comments/{id}` - Get comment by ID
- `GET /api/comments/post/{blogPostId}` - Get comments for a post
- `GET /api/comments/post/{blogPostId}/approved` - Get approved comments
- `POST /api/comments` - Create new comment
- `PUT /api/comments/{id}` - Update comment
- `DELETE /api/comments/{id}` - Delete comment
- `POST /api/comments/{id}/approve` - Approve comment
- `POST /api/comments/{id}/reject` - Reject comment

## 🎯 Business Rules

### Data Integrity
- ✅ **Unique Email Addresses**: Authors must have unique email addresses
- ✅ **Unique Category Names**: Categories must have unique names
- ✅ **Referential Integrity**: Proper validation for delete operations

### Workflow Management
- ✅ **Publishing Workflow**: Blog posts can be published/unpublished
- ✅ **Comment Approval**: Comments require approval before being visible to public

## 🧪 CQRS Implementation

This project implements CQRS using MediatR for clear separation of concerns:

### Commands (Write Operations)
```csharp
// Example: Creating an author
var command = new CreateAuthorCommand("John", "Doe", "john@example.com");
var author = await mediator.Send(command);
```

### Queries (Read Operations)
```csharp
// Example: Getting all authors
var query = new GetAllAuthorsQuery();
var authors = await mediator.Send(query);
```

### Benefits
- 🎯 **Single Responsibility**: Each handler has one specific purpose
- 📈 **Scalability**: Read and write operations can be optimized independently
- 🧪 **Testability**: Easy to unit test individual handlers
- 🔧 **Maintainability**: Clear structure and separation of concerns

## 📂 Project Structure

```
BlogSite/
├── 📄 BlogSite.sln                    # Solution file
├── 📁 src/
│   ├── 📁 BlogSite.Domain/            # Domain entities and business logic
│   ├── 📁 BlogSite.Application/       # CQRS commands/queries, DTOs, services
│   │   ├── 📁 Commands/               # Write operations
│   │   ├── 📁 Queries/                # Read operations
│   │   └── 📁 DTOs/                   # Data transfer objects
│   ├── 📁 BlogSite.Infrastructure/    # Data access and repositories
│   └── 📁 BlogSite.API/              # Web API controllers and configuration
├── 📄 API_SUMMARY.md                  # Detailed API documentation
├── 📄 CQRS_IMPLEMENTATION.md          # CQRS pattern documentation
└── 📄 COMPREHENSIVE_API_DOCUMENTATION.md # Complete API reference
```

## 🔮 Production Readiness

### Immediate Next Steps
- 🔐 **Authentication & Authorization**: Add JWT or OAuth2
- 🗄️ **Production Database**: Configure SQL Server or PostgreSQL
- ⚡ **Caching**: Implement Redis for query caching
- 🛡️ **Rate Limiting**: Add API rate limiting
- 📊 **Monitoring**: Application insights and health checks

### Advanced Features
- 🧪 **Unit & Integration Tests**: Comprehensive test coverage
- 🚀 **CI/CD Pipeline**: Automated build and deployment
- 📝 **Logging**: Structured logging with Serilog
- 🔍 **API Versioning**: Support multiple API versions

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 📚 Additional Documentation

- 📖 [API Summary](API_SUMMARY.md) - Detailed API overview
- 🔄 [CQRS Implementation](CQRS_IMPLEMENTATION.md) - CQRS pattern details
- 📋 [Comprehensive API Documentation](COMPREHENSIVE_API_DOCUMENTATION.md) - Complete API reference

---

**Built with ❤️ using Clean Architecture and CQRS patterns**