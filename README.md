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

### OutboxMessage Table Migration (Transactional Outbox)

If you are using the transactional outbox pattern, you need to create the OutboxMessages table in your PostgreSQL database.

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

## API Documentation & Usage

All API usage, dispatcher pattern, request/response format, and operation mapping documentation are now located in the `Docs/` directory at the project root. Please refer to the following files for detailed guides and examples:

- `Docs/API_TESTING_GUIDE.md` — End-to-end API testing and usage examples
- `Docs/DISPATCHER_USAGE_EXAMPLES.md` — Single dispatcher endpoint usage and request/response format
- `Docs/GENERIC_RESPONSE_GUIDE.md` — Generic response structure and error handling
- `Docs/PREFIX_BASED_DISPATCHER_USAGE.md` — Operation/class name mapping and dispatcher conventions
- `Docs/BASERESPONSE_MIGRATION_SUMMARY.md` — Migration summary and response consistency

### Dispatcher Endpoint
- `POST /api/dispatcher` — Dynamic operation dispatcher (see Docs/DISPATCHER_USAGE_EXAMPLES.md)

### Request Format
All requests use the same format (see Docs/DISPATCHER_USAGE_EXAMPLES.md):
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
Clean-Architecture/
├── BlogApp.sln                 # Solution file
├── BlogApp.API/                # Web API Application
│   ├── Api/                   # API Layer
│   ├── Application/           # Application Layer
│   ├── Core/                  # Core Layer
│   ├── Infrastructure/        # Infrastructure Layer
│   └── ...
├── BlogApp-Angular/            # Angular Frontend
│   ├── src/
│   └── ...
├── Docs/                       # All API and usage documentation
│   ├── API_TESTING_GUIDE.md
│   ├── DISPATCHER_USAGE_EXAMPLES.md
│   ├── GENERIC_RESPONSE_GUIDE.md
│   ├── PREFIX_BASED_DISPATCHER_USAGE.md
│   └── BASERESPONSE_MIGRATION_SUMMARY.md
├── PROJECT_DOCUMENTATION.md    # Full project documentation
└── README.md                   # This file
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

## Changelog

### Version 3.0.0
- Replaced MVC with Angular frontend
- Implemented CQRS pattern with separate databases
- Added PostgreSQL for commands and MongoDB for queries
- Implemented Repository and Unit of Work patterns
- Added dynamic dispatcher endpoint
- Enhanced project architecture with Clean Architecture

### Version 2.0.0
- Added Web API project
- JWT authentication
- Swagger documentation
- RESTful API endpoints
- Enhanced project structure

### Version 1.0.0
- Initial MVC release
- Basic blog functionality
- User authentication
- Comment system
- Search and filtering
- Responsive design 