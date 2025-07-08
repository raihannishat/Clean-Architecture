# BlogSite API - ASP.NET Core 8.0 with Clean Architecture

## Project Overview

Successfully created a comprehensive blog management API using ASP.NET Core 8.0 following clean architecture principles. The API provides full CRUD operations for managing blog posts, authors, categories, and comments.

## Architecture

### Clean Architecture Implementation
- **Domain Layer** (`BlogSite.Domain`): Core business entities and domain logic
- **Application Layer** (`BlogSite.Application`): Business logic, DTOs, interfaces, and services
- **Infrastructure Layer** (`BlogSite.Infrastructure`): Data access, repositories, and Entity Framework implementation
- **API Layer** (`BlogSite.API`): RESTful controllers, dependency injection, and configuration

### Key Design Patterns
- Repository Pattern for data access abstraction
- Dependency Inversion Principle throughout all layers
- AutoMapper for object-to-object mapping
- Service Layer pattern for business logic encapsulation

## Features Implemented

### Entities
- **Author**: Manages blog authors with personal information and blog post relationships
- **Category**: Organizes blog posts into categories
- **BlogPost**: Core content entity with publishing workflow
- **Comment**: User comments with approval system

### API Endpoints

#### Authors (`/api/authors`)
- `GET /api/authors` - Get all authors
- `GET /api/authors/{id}` - Get author by ID
- `GET /api/authors/email/{email}` - Get author by email
- `POST /api/authors` - Create new author
- `PUT /api/authors/{id}` - Update author
- `DELETE /api/authors/{id}` - Delete author

#### Categories (`/api/categories`)
- `GET /api/categories` - Get all categories
- `GET /api/categories/{id}` - Get category by ID
- `GET /api/categories/name/{name}` - Get category by name
- `POST /api/categories` - Create new category
- `PUT /api/categories/{id}` - Update category
- `DELETE /api/categories/{id}` - Delete category

#### Blog Posts (`/api/blogposts`)
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

#### Comments (`/api/comments`)
- `GET /api/comments` - Get all comments
- `GET /api/comments/{id}` - Get comment by ID
- `GET /api/comments/post/{blogPostId}` - Get comments for a post
- `GET /api/comments/post/{blogPostId}/approved` - Get approved comments for a post
- `POST /api/comments` - Create new comment
- `PUT /api/comments/{id}` - Update comment
- `DELETE /api/comments/{id}` - Delete comment
- `POST /api/comments/{id}/approve` - Approve comment
- `POST /api/comments/{id}/reject` - Reject comment

## Technology Stack

### Core Framework
- ASP.NET Core 8.0
- Entity Framework Core 9.0.6 with SQLite provider
- AutoMapper 15.0.0

### Development Tools
- Swagger/OpenAPI for API documentation
- Dependency Injection container
- CORS configuration for frontend integration

### Database
- SQLite for development (easily portable)
- Automatic database creation and seeding
- Code-first migrations support

## Key Features

### Business Logic
- **Email Uniqueness**: Authors must have unique email addresses
- **Category Name Uniqueness**: Categories must have unique names
- **Publishing Workflow**: Blog posts can be published/unpublished
- **Comment Approval**: Comments require approval before being visible
- **Referential Integrity**: Proper validation for delete operations

### Data Validation
- Comprehensive DTOs for create/update operations
- Model validation with proper error responses
- Null safety throughout the application

### Error Handling
- Structured exception handling in all controllers
- Appropriate HTTP status codes
- Detailed logging for debugging

### API Documentation
- Comprehensive Swagger documentation
- XML comments for all endpoints
- OpenAPI specification compliance

## Project Structure

```
BlogSite/
├── src/
│   ├── BlogSite.Domain/          # Entities and domain logic
│   ├── BlogSite.Application/     # Services, DTOs, and interfaces
│   ├── BlogSite.Infrastructure/  # Data access and repositories
│   └── BlogSite.API/            # Controllers and configuration
└── tests/                       # Test projects (structure created)
```

## Configuration

### Database
- SQLite database (`BlogSite.db`) created automatically
- Seed data includes sample author, category, and blog post
- Entity relationships properly configured

### Logging
- Console logging configured
- Entity Framework command logging
- Structured logging for production readiness

### CORS
- Configured for development with permissive policy
- Ready for production configuration

## Running the Application

1. **Build the solution**:
   ```bash
   dotnet build
   ```

2. **Run the API**:
   ```bash
   cd src/BlogSite.API
   dotnet run
   ```

3. **Access the API**:
   - API Base URL: `http://localhost:5165`
   - Swagger UI: `http://localhost:5165` (served at root)
   - API Documentation: Interactive Swagger interface

## Testing

The API has been successfully tested with:
- ✅ All GET endpoints returning correct data
- ✅ POST endpoints creating new resources
- ✅ Database seeding working correctly
- ✅ AutoMapper configurations functional
- ✅ Dependency injection properly configured
- ✅ Error handling working as expected

## Next Steps for Production

1. **Security**: Add authentication and authorization
2. **Database**: Configure production database (SQL Server, PostgreSQL)
3. **Caching**: Implement caching strategies
4. **Rate Limiting**: Add API rate limiting
5. **Monitoring**: Add application monitoring and health checks
6. **Testing**: Implement unit and integration tests
7. **CI/CD**: Set up continuous integration and deployment

## Conclusion

The BlogSite API successfully demonstrates clean architecture principles with ASP.NET Core 8.0, providing a solid foundation for a production blog management system. The implementation includes comprehensive CRUD operations, proper data relationships, business logic validation, and excellent API documentation.