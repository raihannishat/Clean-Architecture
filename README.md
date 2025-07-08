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

## � Dynamic API System

This API uses a **Dynamic Dispatch System** with **ASP.NET Core Minimal APIs** that automatically routes requests to appropriate CQRS handlers without the need for traditional controllers.

### 🎯 Dynamic Endpoint Structure

Instead of predefined endpoints, the API uses a dynamic routing system:

**Base Route**: `/api/dispatch/{action}`

**Usage Examples**:
- `POST /api/dispatch/getallauthors` - Get all authors
- `POST /api/dispatch/createauthor` - Create new author
- `POST /api/dispatch/getauthorbyid` - Get author by ID
- `POST /api/dispatch/getallcategories` - Get all categories
- `POST /api/dispatch/createcategory` - Create new category
- `POST /api/dispatch/getpublishedblogposts` - Get published blog posts

### ⚡ Auto-Discovery Features

- **Automatic Type Resolution**: Dynamically finds and executes CQRS handlers
- **Convention-Based Routing**: Action names automatically map to handler classes
- **Smart Case Conversion**: `getallauthors` → `GetAllAuthorsQuery`
- **Entity Recognition**: Automatically detects entity types (Author, BlogPost, Category, Comment)
- **Operation Type Detection**: Automatically determines if it's a Query or Command

### 📝 Request Format

All requests use JSON payload format:

```json
POST /api/dispatch/{action}
Content-Type: application/json

{
  // Request parameters specific to the action
}
```

### 🔍 Available Actions

The system automatically discovers and supports all CQRS operations. Use the Swagger documentation at the root URL (`/`) to explore all available endpoints and their request/response formats.

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