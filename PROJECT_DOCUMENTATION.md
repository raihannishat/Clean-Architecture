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

## 📁 Project Structure

```
example/
├── BlogApp.API/                    # Backend API
│   ├── Api/                       # API Layer
│   │   ├── Configuration/         # FastEndpoints Configuration
│   │   └── Endpoints/             # API Endpoints
│   ├── Application/               # Application Layer
│   │   ├── CQRS/                 # CQRS Implementation
│   │   │   ├── Commands/         # Command Handlers
│   │   │   ├── Handlers/         # Query Handlers
│   │   │   ├── ICommand.cs       # Command Interface
│   │   │   ├── IQuery.cs         # Query Interface
│   │   │   └── Mediator.cs       # Mediator Implementation
│   │   └── Features/             # Feature Modules
│   │       ├── Auth/             # Authentication
│   │       ├── Blog/             # Blog Management
│   │       └── Comment/          # Comment System
│   ├── Core/                     # Core Layer
│   │   ├── Entities/             # Domain Entities
│   │   ├── Exceptions/           # Custom Exceptions
│   │   └── Interfaces/           # Core Interfaces
│   └── Infrastructure/           # Infrastructure Layer
│       ├── Persistence/          # Data Access
│       │   ├── Contexts/         # DbContexts
│       │   ├── Repositories/     # Repository Pattern
│       │   ├── UnitOfWork/       # Unit of Work Pattern
│       │   └── Factories/        # Factory Pattern
│       └── Services/             # External Services
└── BlogApp-Angular/              # Angular Frontend
    ├── src/
    │   ├── app/
    │   │   ├── features/         # Feature Modules
    │   │   │   ├── auth/         # Authentication
    │   │   │   └── blog/         # Blog Management
    │   │   ├── models/           # TypeScript Models
    │   │   ├── services/         # Angular Services
    │   │   └── interceptors/     # HTTP Interceptors
    │   └── styles.scss           # Global Styles
    ├── package.json              # Dependencies
    └── angular.json              # Angular Configuration
```

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
   ```bash
   dotnet ef database update
   ```

4. **Run the API**
   ```bash
   dotnet run
   ```

### Frontend Setup
1. **Navigate to Angular project**
   ```bash
   cd BlogApp-Angular
   ```

2. **Install dependencies**
   ```bash
   npm install
   ```

3. **Run the application**
   ```bash
   ng serve
   ```

4. **Access the application**
   - Angular: `http://localhost:4200`
   - API: `https://localhost:7001`
   - Swagger: `https://localhost:7001/swagger`

## 📡 API Usage

### Dispatcher Endpoint
All API operations go through the single dispatcher endpoint:
```
POST /api/dispatcher
```

### Request Format
```json
{
  "operation": "OperationName",
  "data": {
    // Operation-specific parameters
  }
}
```

### Available Operations

#### Authentication
- `LoginCommand` - User login
- `RegisterCommand` - User registration

#### Blog Management
- `CreateBlogPostCommand` - Create new blog post
- `GetBlogPostsQuery` - Get all published posts
- `GetBlogPostBySlugQuery` - Get post by slug
- `GetCategoriesQuery` - Get all categories
- `GetTagsQuery` - Get all tags
- `SearchPostsQuery` - Search posts

#### Comments
- `CreateCommentCommand` - Create new comment
- `GetCommentsQuery` - Get comments for a post

### Response Format
All responses follow the `BaseResponse<T>` structure:
```json
{
  "isSuccess": true,
  "message": "Operation completed successfully",
  "data": { /* operation-specific data */ },
  "errors": [],
  "statusCode": 200
}
```

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