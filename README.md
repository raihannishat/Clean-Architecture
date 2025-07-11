# BlogApp - ASP.NET Core 8.0 Blog Application

A modern, feature-rich blog application built with ASP.NET Core 8.0, Entity Framework Core, and Angular. This solution includes both a Web API backend and an Angular frontend.

## Projects

### BlogApp.API (Web API)
- **Type**: ASP.NET Core Web API
- **Purpose**: RESTful API for blog operations
- **Features**: JWT authentication, CRUD operations, JSON responses, Swagger documentation, CQRS pattern, Repository pattern, Unit of Work pattern

### BlogApp-Angular (Frontend)
- **Type**: Angular 17 Application
- **Purpose**: Modern single-page application frontend
- **Features**: User authentication, blog management, comments, search, responsive design, dynamic dispatcher service

## Features

### Core Features
- **User Authentication & Authorization** - Built-in user registration and login system
- **Blog Post Management** - Create, edit, delete, and publish blog posts
- **Rich Content Editor** - Support for HTML content with formatting
- **Categories & Tags** - Organize posts with categories and tags
- **Comment System** - Nested comments with reply functionality
- **Search Functionality** - Search posts by title, content, or summary
- **Responsive Design** - Mobile-friendly Bootstrap 5 interface

### Advanced Features
- **SEO-Friendly URLs** - Automatic slug generation for posts
- **View Count Tracking** - Track post popularity
- **Pagination** - Efficient post listing with pagination
- **Image Support** - Featured images for blog posts
- **User Profiles** - Extended user information and bio
- **Admin Dashboard** - Manage posts, categories, and tags
- **JWT Authentication** - Secure API access with JSON Web Tokens
- **Swagger Documentation** - Interactive API documentation

## Technology Stack

- **Backend**: ASP.NET Core 8.0
- **Database**: PostgreSQL (Commands) + MongoDB (Queries)
- **ORM**: Entity Framework Core 8.0 + MongoDB.EntityFrameworkCore
- **Authentication**: ASP.NET Core Identity + JWT Bearer
- **Frontend**: Angular 17, TypeScript, SCSS
- **API Documentation**: Swagger/OpenAPI
- **Package Management**: NuGet (Backend) + npm (Frontend)
- **Architecture**: Clean Architecture, CQRS, Repository Pattern, Unit of Work Pattern

## Prerequisites

- .NET 8.0 SDK
- Node.js 18+ and npm
- PostgreSQL (for commands)
- MongoDB (for queries)
- Visual Studio 2022 or VS Code

## Installation & Setup

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd example
   ```

2. **Restore NuGet packages**
   ```bash
   dotnet restore
   ```

3. **Update database connection strings** (if needed)
   - Open `BlogApp.API/appsettings.json` for API project
   - Modify the `CommandConnection` (PostgreSQL) and `QueryConnection` (MongoDB) strings

4. **Run database migrations**
   ```bash
   # For API project
   cd BlogApp.API
   dotnet ef database update
   ```

5. **Install frontend dependencies**
   ```bash
   cd ../BlogApp-Angular
   npm install
   ```

6. **Run the applications**
   ```bash
   # Run API application
   cd ../BlogApp.API
   dotnet run
   
   # Run Angular application (in another terminal)
   cd ../BlogApp-Angular
   ng serve
   ```

7. **Access the applications**
   - Angular Application: `http://localhost:4200`
   - API Application: `https://localhost:7001` or `http://localhost:7000`
   - Swagger Documentation: `https://localhost:7001/swagger`

## Default Users

The applications come with pre-configured users:

- **Admin User**
  - Email: `admin@blogapp.com`
  - Password: `Admin123!`
  - Role: Admin

- **Demo User**
  - Email: `demo@blogapp.com`
  - Password: `Demo123!`
  - Role: User

## API Endpoints

The application uses a **Single Dispatcher Endpoint** pattern where all operations are handled through one dynamic endpoint.

### Dispatcher Endpoint
- `POST /api/dispatcher` - Dynamic operation dispatcher

### Available Operations

#### Authentication Operations
- `LoginCommand` - User login
- `RegisterCommand` - User registration

#### Blog Operations
- `CreateBlogPostCommand` - Create new blog post
- `GetBlogPostsQuery` - Get all published posts
- `GetBlogPostBySlugQuery` - Get post by slug
- `GetCategoriesQuery` - Get all categories
- `GetTagsQuery` - Get all tags
- `SearchPostsQuery` - Search posts

#### Comment Operations
- `CreateCommentCommand` - Create new comment
- `GetCommentsQuery` - Get comments for a post

### Request Format
All requests use the same format:
```json
{
  "operation": "OperationName",
  "data": {
    // Operation-specific data
  }
}
```

### Authentication
Protected operations require a valid JWT token in the Authorization header:
```
Authorization: Bearer {your-jwt-token}
```

## Project Structure

```
example/
├── BlogApp.sln                 # Solution file
├── BlogApp.API/               # Web API Application
│   ├── Api/                   # API Layer
│   │   ├── Configuration/     # FastEndpoints Configuration
│   │   └── Endpoints/         # API Endpoints
│   ├── Application/           # Application Layer
│   │   ├── CQRS/             # CQRS Implementation
│   │   │   ├── Commands/     # Command Handlers
│   │   │   ├── Handlers/     # Query Handlers
│   │   │   ├── ICommand.cs   # Command Interface
│   │   │   ├── IQuery.cs     # Query Interface
│   │   │   └── Mediator.cs   # Mediator Implementation
│   │   └── Features/         # Feature Modules
│   │       ├── Auth/         # Authentication
│   │       ├── Blog/         # Blog Management
│   │       └── Comment/      # Comment System
│   ├── Core/                 # Core Layer
│   │   ├── Entities/         # Domain Entities
│   │   ├── Exceptions/       # Custom Exceptions
│   │   └── Interfaces/       # Core Interfaces
│   └── Infrastructure/       # Infrastructure Layer
│       ├── Persistence/      # Data Access
│       │   ├── Contexts/     # DbContexts
│       │   ├── Repositories/ # Repository Pattern
│       │   ├── UnitOfWork/   # Unit of Work Pattern
│       │   └── Factories/    # Factory Pattern
│       └── Services/         # External Services
└── BlogApp-Angular/          # Angular Frontend
    ├── src/
    │   ├── app/
    │   │   ├── features/     # Feature Modules
    │   │   │   ├── auth/     # Authentication
    │   │   │   └── blog/     # Blog Management
    │   │   ├── models/       # TypeScript Models
    │   │   ├── services/     # Angular Services
    │   │   └── interceptors/ # HTTP Interceptors
    │   └── styles.scss       # Global Styles
    ├── package.json          # Dependencies
    └── angular.json          # Angular Configuration
```

## Database Schema

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

## API Usage Examples

### Login
```bash
curl -X POST "https://localhost:7001/api/dispatcher" \
  -H "Content-Type: application/json" \
  -d '{
    "operation": "LoginCommand",
    "data": {
      "email": "admin@blogapp.com",
      "password": "Admin123!"
    }
  }'
```

### Get Posts
```bash
curl -X POST "https://localhost:7001/api/dispatcher" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer {your-jwt-token}" \
  -d '{
    "operation": "GetBlogPostsQuery",
    "data": {}
  }'
```

### Create Post
```bash
curl -X POST "https://localhost:7001/api/dispatcher" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer {your-jwt-token}" \
  -d '{
    "operation": "CreateBlogPostCommand",
    "data": {
      "title": "My New Post",
      "content": "Post content here...",
      "summary": "Brief summary",
      "categoryId": 1,
      "tagIds": [1, 2],
      "isPublished": true
    }
  }'
```

### Get Post by Slug
```bash
curl -X POST "https://localhost:7001/api/dispatcher" \
  -H "Content-Type: application/json" \
  -d '{
    "operation": "GetBlogPostBySlugQuery",
    "data": {
      "slug": "my-blog-post-slug"
    }
  }'
```

## Customization

### Adding New Categories
1. Add category data in `DbInitializer.cs`
2. Or create through the application interface

### Adding New Tags
1. Add tag data in `DbInitializer.cs`
2. Or create through the application interface

### Styling (Angular)
- Modify `BlogApp-Angular/src/styles.scss` for global styles
- Update component-specific styles in feature modules
- Use Angular Material or Bootstrap for UI components

### API Extensions
- Extend `IBlogService` interface for new functionality
- Add new controllers for additional features
- Create new DTOs as needed

## Deployment

### Local Deployment
1. Ensure SQL Server is running
2. Update connection strings in `appsettings.json` files
3. Run `dotnet ef database update` for both projects
4. Run `dotnet run` for both projects

### Production Deployment
1. Set up production PostgreSQL and MongoDB databases
2. Update connection strings
3. Configure environment variables
4. Set up reverse proxy (nginx/Apache)
5. Use `dotnet publish` for optimized API builds
6. Use `ng build --prod` for optimized Angular builds

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests if applicable
5. Submit a pull request

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Support

For support and questions:
- Create an issue in the repository
- Contact the development team
- Check the documentation
