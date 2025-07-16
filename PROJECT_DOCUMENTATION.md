# BlogApp - Complete Project Documentation

A modern blog application built with ASP.NET Core 8.0 (Clean Architecture + CQRS) and Angular 17, featuring a dynamic dispatcher pattern for seamless API communication.

## 🏗️ Architecture Overview

### Backend (BlogApp.API)
- **Framework**: ASP.NET Core 8.0
- **Architecture**: Clean Architecture with CQRS pattern
- **Databases**: PostgreSQL (Commands) + MongoDB (Queries)
- **ORM**: Entity Framework Core + MongoDB.EntityFrameworkCore
- **Authentication**: ASP.NET Core Identity + JWT Bearer
- **API Pattern**: Single Dynamic Dispatcher Endpoint
- **Validation**: FluentValidation
- **Dependency Injection**: Auto-Register pattern

### Frontend (BlogApp-Angular)
- **Framework**: Angular 17
- **Language**: TypeScript
- **Styling**: SCSS
- **API Communication**: Dynamic Dispatcher Service
- **State Management**: RxJS Observables
- **Routing**: Angular Router

## 📁 Project Structure (Detailed)

```
Clean-Architecture/
├── BlogApp.API/                # Web API Application (Backend)
│   ├── Api/                   # API Layer (entry point, endpoints, middleware, config)
│   │   ├── Configuration/     # FastEndpoints, Swagger, and other API configs
│   │   ├── Endpoints/         # API endpoint definitions (controllers/handlers)
│   │   └── Middleware/        # Global exception and custom middleware
│   ├── Application/           # Application Layer (CQRS, business logic, features)
│   │   ├── CQRS/              # Core CQRS abstractions (ICommand, IQuery, Mediator, Handlers)
│   │   ├── Common/            # Shared application-level utilities (BaseResponse, interfaces)
│   │   ├── Extensions/        # Service registration and DI extensions
│   │   └── Features/          # Feature modules (by domain)
│   │       ├── Auth/          # Authentication (DTOs, Commands, Mapping)
│   │       ├── Blog/          # Blog management (Commands, Queries, DTOs, Validators, Mapping)
│   │       └── Comment/       # Comment system (Commands, Queries, DTOs, Validators, Mapping)
│   ├── Core/                  # Core Layer (domain entities, interfaces)
│   │   ├── Entities/          # Domain models (BlogPost, Comment, User, etc.)
│   │   └── Interfaces/        # Core business interfaces (e.g., IAuthService, IBlogService)
│   ├── Infrastructure/        # Infrastructure Layer (data access, services)
│   │   ├── Persistence/       # Database contexts, repositories, unit of work, factories
│   │   │   ├── Contexts/      # EF Core and MongoDB DbContexts
│   │   │   ├── Repositories/  # Repository pattern implementations and interfaces
│   │   │   ├── UnitOfWork/    # Unit of Work pattern implementations and interfaces
│   │   │   └── Factories/     # DbContext factories for DI/testing
│   │   └── Services/          # Infrastructure services (e.g., OutboxProcessor)
│   ├── Properties/            # Launch settings and project properties
│   ├── bin/                   # Build output (ignored in VCS)
│   ├── obj/                   # Build artifacts (ignored in VCS)
│   └── appsettings.json       # Main API configuration
├── BlogApp-Angular/           # Angular Frontend Application
│   ├── src/
│   │   ├── app/
│   │   │   ├── features/      # Feature modules (auth, blog, etc.)
│   │   │   ├── models/        # TypeScript interfaces/models (BlogPost, Comment, etc.)
│   │   │   ├── services/      # Angular services (API, dispatcher, etc.)
│   │   │   ├── interceptors/  # HTTP interceptors (e.g., auth)
│   │   │   └── components/    # UI components (navigation, dispatcher example, etc.)
│   │   ├── styles.scss        # Global styles
│   │   └── ...                # Other Angular config files
│   ├── package.json           # Frontend dependencies
│   └── angular.json           # Angular CLI config
├── Docs/                      # All API and usage documentation (Markdown)
│   ├── API_TESTING_GUIDE.md
│   ├── DISPATCHER_USAGE_EXAMPLES.md
│   ├── GENERIC_RESPONSE_GUIDE.md
│   ├── PREFIX_BASED_DISPATCHER_USAGE.md
│   └── BASERESPONSE_MIGRATION_SUMMARY.md
├── PROJECT_DOCUMENTATION.md   # Full project documentation (this file)
└── README.md                  # Main readme
```

---

### **Folder Responsibilities (Summary)**

- **Api/**: API entry point, endpoint definitions, middleware, and configuration.
- **Application/CQRS/**: CQRS interfaces and mediator pattern (ICommand, IQuery, Handlers).
- **Application/Features/**: Each domain feature (Auth, Blog, Comment) has its own folder, containing Commands, Queries, DTOs, Validators, and Mapping.
- **Core/Entities/**: Domain models/entities (pure business objects).
- **Core/Interfaces/**: Core business logic interfaces (service contracts).
- **Infrastructure/Persistence/**: Data access, repository, unit of work, and context management.
- **Infrastructure/Services/**: Background services and infrastructure-specific logic.
- **Docs/**: All API, usage, and migration documentation in Markdown format.
- **BlogApp-Angular/src/app/**: Angular app source code, organized by features, models, services, and components.

## 📚 API Documentation & Usage

All API usage, dispatcher pattern, request/response format, and operation mapping documentation are now located in the `Docs/` directory at the project root. Please refer to the following files for detailed guides and examples:

- `Docs/API_TESTING_GUIDE.md` — End-to-end API testing and usage examples
- `Docs/DISPATCHER_USAGE_EXAMPLES.md` — Single dispatcher endpoint usage and request/response format
- `Docs/GENERIC_RESPONSE_GUIDE.md` — Generic response structure and error handling
- `Docs/PREFIX_BASED_DISPATCHER_USAGE.md` — Operation/class name mapping and dispatcher conventions
- `Docs/BASERESPONSE_MIGRATION_SUMMARY.md` — Migration summary and response consistency

## 🚀 Key Features

### Backend Features
- **Dynamic Dispatcher Endpoint**: Single endpoint (`/api/dispatcher`) handles all operations
- **CQRS Pattern**: Separate command and query handlers with different databases
- **Repository Pattern**: Generic repositories with entity constraints
- **Unit of Work Pattern**: Transaction management and consistency
- **Auto-Register**: Automatic dependency injection registration
- **BaseResponse<T>**: Consistent response structure across all operations
- **Global Exception Handling**: Centralized error handling middleware
- **JWT Authentication**: Secure token-based authentication
- **FluentValidation**: Comprehensive input validation

### Frontend Features
- **Dynamic Dispatcher Service**: Proxy-based service for all API operations
- **Type-Safe Operations**: Full TypeScript support with generics
- **Responsive Design**: Mobile-friendly interface
- **Error Handling**: Comprehensive error management
- **Authentication**: JWT token management with interceptors
- **Feature Modules**: Organized by domain features

## 🛠️ Setup Instructions

### Prerequisites
- .NET 8.0 SDK
- Node.js 18+ and npm
- PostgreSQL (for commands)
- MongoDB (for queries)
- Visual Studio 2022 or VS Code

### Backend Setup
1. **Clone and navigate to API project**
   ```bash
   cd BlogApp.API
   ```

2. **Update connection strings** in `appsettings.json`
   ```json
   {
     "ConnectionStrings": {
       "CommandConnection": "Host=localhost;Database=BlogAppCommands;Username=postgres;Password=password",
       "QueryConnection": "mongodb://localhost:27017/BlogAppQueries"
     }
   }
   ```

3. **Run database migrations**

#### OutboxMessage Table Migration (Transactional Outbox)
If you are using the transactional outbox pattern, create the OutboxMessages table in your PostgreSQL database.

**Manual SQL:**
```sql
CREATE TABLE "OutboxMessages" (
    "Id" uuid PRIMARY KEY,
    "Type" varchar(200) NOT NULL,
    "Payload" text NOT NULL,
    "OccurredOn" timestamp NOT NULL DEFAULT NOW(),
    "ProcessedOn" timestamp NULL
);
```

**EF Core:**
```sh
dotnet ef migrations add AddOutboxMessagesTable --project BlogApp.API
dotnet ef database update --project BlogApp.API
```

---

### How PostgreSQL to MongoDB Sync Works (Transactional Outbox Pattern)

When using the Transactional Outbox pattern, your data flows from PostgreSQL (write DB) to MongoDB (read DB) in the following steps:

#### 1. Command Handler: Data Write & Outbox Message Creation
- Each command handler writes the main data to PostgreSQL (CommandDbContext).
- In the same transaction, it also writes an OutboxMessage entity to the OutboxMessages table.
- The OutboxMessage contains:
  - Type: The command type name (e.g., "CreateBlogPostCommand")
  - Payload: The serialized command object (JSON)
  - OccurredOn: When the event happened
  - ProcessedOn: When it was synced to MongoDB (initially null)

Example:
```csharp
await _dbContext.BlogPosts.AddAsync(new BlogPost { ... }, cancellationToken);
await _outboxService.AddAsync(nameof(CreateBlogPostCommand), command, cancellationToken);
await _dbContext.SaveChangesAsync(cancellationToken);
```

#### 2. OutboxMessages Table Stores Pending Events
- Each new command creates a row in OutboxMessages.
- These rows are the "pending events" to be synced to MongoDB.

#### 3. Background Worker (OutboxProcessor) Runs
- The OutboxProcessor background service polls the OutboxMessages table every second.
- It selects messages where ProcessedOn is null (not yet synced).

Example:
```csharp
var messages = await _dbContext.OutboxMessages
    .Where(m => m.ProcessedOn == null)
    .OrderBy(m => m.OccurredOn)
    .Take(10)
    .ToListAsync(stoppingToken);
```

#### 4. Worker Syncs Each Message to MongoDB
- For each message:
  - Uses Type as the MongoDB collection name (e.g., "CreateBlogPostCommand")
  - Converts Payload JSON to BsonDocument
  - Inserts into the corresponding MongoDB collection

Example:
```csharp
var collection = _mongoDatabase.GetCollection<BsonDocument>(message.Type);
var doc = BsonDocument.Parse(message.Payload);
await collection.InsertOneAsync(doc, cancellationToken: stoppingToken);
```

#### 5. On Success, ProcessedOn is Set
- If insert succeeds, sets ProcessedOn to current timestamp and saves changes.
- This marks the message as processed.

Example:
```csharp
message.ProcessedOn = DateTime.UtcNow;
await _dbContext.SaveChangesAsync(stoppingToken);
```

#### 6. On Failure, Message Remains Pending
- If insert fails, ProcessedOn remains null.

---

## 🔗 Further Documentation

For detailed API usage, dispatcher pattern, request/response format, and operation mapping, see the Docs/ directory:

- `Docs/API_TESTING_GUIDE.md`
- `Docs/DISPATCHER_USAGE_EXAMPLES.md`
- `Docs/GENERIC_RESPONSE_GUIDE.md`
- `Docs/PREFIX_BASED_DISPATCHER_USAGE.md`
- `Docs/BASERESPONSE_MIGRATION_SUMMARY.md`

---

## 🔧 Development Guidelines

### Adding New Operations

#### 1. Create Command/Query
```csharp
// In Application/Features/YourFeature/Commands/
public class YourCommand : ICommand<BaseResponse<YourResult>>
{
    public string Property { get; set; } = string.Empty;
}

public class YourCommandHandler : ICommandHandler<YourCommand, BaseResponse<YourResult>>
{
    private readonly ICommandUnitOfWork _unitOfWork;

    public YourCommandHandler(ICommandUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<BaseResponse<YourResult>> HandleAsync(YourCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            // Your business logic here
            var result = new YourResult();
            
            return BaseResponse<YourResult>.Success(result, "Operation successful");
        }
        catch (Exception ex)
        {
            return BaseResponse<YourResult>.Failure($"Operation failed: {ex.Message}", 500);
        }
    }
}
```

#### 2. Use in Angular (Automatic)
```typescript
// Works immediately - no service updates needed!
this.dispatcher.dynamic.YourCommand({ property: 'value' })
  .subscribe(result => {
    console.log('Result:', result);
  });
```

### Database Operations

#### Commands (PostgreSQL)
- Use `ICommandUnitOfWork` for write operations
- Commands should be in the Commands folder
- Return `BaseResponse<T>` for consistency

#### Queries (MongoDB)
- Use `IQueryUnitOfWork` for read operations
- Queries should be in the Queries folder
- Return `BaseResponse<T>` for consistency

### Validation
- Use FluentValidation for input validation
- Place validators in the same file as commands/queries
- Return validation errors in `BaseResponse.Errors`

## 🔒 Security

### Authentication
- JWT Bearer tokens for API access
- Tokens expire after 7 days
- Protected operations require valid tokens

### Authorization
- Role-based access control
- User-specific data isolation
- Input validation and sanitization

## 📊 Database Schema

### Core Entities
- **ApplicationUser** - Extended Identity user with profile information
- **BlogPost** - Blog posts with title, content, metadata
- **Category** - Post categories for organization
- **Tag** - Post tags for better discoverability
- **Comment** - User comments with nested replies
- **BlogPostTag** - Many-to-many relationship between posts and tags

### Key Relationships
- User → BlogPosts (One-to-Many)
- User → Comments (One-to-Many)
- Category → BlogPosts (One-to-Many)
- BlogPost → Comments (One-to-Many)
- BlogPost ↔ Tags (Many-to-Many via BlogPostTag)
- Comment → Replies (Self-referencing)

## 🧪 Testing

### API Testing
- Use the `/api/dispatcher` endpoint for all operations
- Test with curl, Postman, or the provided JavaScript examples
- All operations return consistent `BaseResponse<T>` format

### Frontend Testing
- Use the dynamic dispatcher service for all API calls
- Leverage TypeScript for type safety
- Implement proper error handling

## 🚀 Deployment

### Backend Deployment
1. Set up production PostgreSQL and MongoDB databases
2. Update connection strings in `appsettings.json`
3. Configure environment variables
4. Use `dotnet publish` for optimized builds
5. Set up reverse proxy (nginx/Apache)

### Frontend Deployment
1. Update API URL in dispatcher service
2. Use `ng build --prod` for optimized builds
3. Deploy to static hosting (Azure Static Web Apps, Netlify, etc.)

## 📚 Additional Documentation

- **API Testing Guide**: `BlogApp.API/API_TESTING_GUIDE.md`
- **Dispatcher Usage**: `BlogApp.API/DISPATCHER_USAGE_EXAMPLES.md`
- **BaseResponse Guide**: `BlogApp.API/GENERIC_RESPONSE_GUIDE.md`
- **Angular Integration**: `BlogApp-Angular/DISPATCHER_INTEGRATION_GUIDE.md`

## 🤝 Contributing

1. Follow the established patterns and architecture
2. Use the dynamic dispatcher for all API operations
3. Implement proper error handling and validation
4. Maintain type safety throughout the application
5. Follow Clean Architecture principles

## 📄 License

This project is licensed under the MIT License.

---

**Note**: This project demonstrates modern web development practices with Clean Architecture, CQRS, and dynamic API patterns. The single dispatcher endpoint and dynamic Angular service provide a scalable and maintainable solution for complex applications. 