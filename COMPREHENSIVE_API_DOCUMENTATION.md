# BlogSite API - Comprehensive Documentation

## Table of Contents
1. [Project Overview](#project-overview)
2. [Architecture](#architecture)
3. [Domain Entities](#domain-entities)
4. [Data Transfer Objects (DTOs)](#data-transfer-objects-dtos)
5. [CQRS Implementation](#cqrs-implementation)
6. [Repository Pattern](#repository-pattern)
7. [REST API Endpoints](#rest-api-endpoints)
8. [Configuration and Middleware](#configuration-and-middleware)
9. [Usage Examples](#usage-examples)
10. [Error Handling](#error-handling)

## Project Overview

BlogSite is a comprehensive blog management API built with ASP.NET Core 8.0 following clean architecture principles and CQRS pattern. The project provides full CRUD operations for managing blog posts, authors, categories, and comments.

### Technology Stack
- **Framework**: ASP.NET Core 8.0
- **Database**: Entity Framework Core 9.0.6 with SQLite
- **Patterns**: Clean Architecture, CQRS with MediatR
- **Mapping**: AutoMapper 15.0.0
- **Documentation**: Swagger/OpenAPI

### Key Features
- Clean Architecture with proper separation of concerns
- CQRS pattern for improved scalability and maintainability
- Repository pattern for data access abstraction
- Comprehensive validation and error handling
- Automatic database creation and seeding
- Full API documentation with Swagger

---

## Architecture

### Layer Structure

```
BlogSite/
├── src/
│   ├── BlogSite.Domain/          # Core business entities
│   ├── BlogSite.Application/     # Business logic, CQRS, DTOs
│   ├── BlogSite.Infrastructure/  # Data access, repositories
│   └── BlogSite.API/            # REST controllers, configuration
```

### Dependencies Flow
- **API** → **Application** → **Domain**
- **API** → **Infrastructure** → **Application**
- **Infrastructure** → **Domain**

---

## Domain Entities

### BaseEntity
All entities inherit from `BaseEntity` providing common properties.

```csharp
public abstract class BaseEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
```

### Author Entity
Represents blog authors with personal information and relationships to blog posts.

```csharp
public class Author : BaseEntity
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;        // Unique constraint
    public string Bio { get; set; } = string.Empty;
    
    // Computed property
    public string FullName => $"{FirstName} {LastName}";
    
    // Navigation properties
    public ICollection<BlogPost> BlogPosts { get; set; } = new List<BlogPost>();
}
```

**Business Rules:**
- Email must be unique across all authors
- FirstName and LastName are required
- Bio is optional

### BlogPost Entity
Core content entity representing blog articles with publishing workflow.

```csharp
public class BlogPost : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public bool IsPublished { get; set; } = false;
    public DateTime? PublishedAt { get; set; }
    
    // Foreign keys
    public Guid AuthorId { get; set; }
    public Guid CategoryId { get; set; }
    
    // Navigation properties
    public Author Author { get; set; } = null!;
    public Category Category { get; set; } = null!;
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
}
```

**Business Rules:**
- Must have an author and category
- PublishedAt is set automatically when IsPublished becomes true
- Content and Summary are required for published posts

### Category Entity
Organizes blog posts into logical categories.

```csharp
public class Category : BaseEntity
{
    public string Name { get; set; } = string.Empty;         // Unique constraint
    public string Description { get; set; } = string.Empty;
    
    // Navigation properties
    public ICollection<BlogPost> BlogPosts { get; set; } = new List<BlogPost>();
}
```

**Business Rules:**
- Name must be unique across all categories
- Description is optional

### Comment Entity
User comments with approval workflow for blog posts.

```csharp
public class Comment : BaseEntity
{
    public string AuthorName { get; set; } = string.Empty;
    public string AuthorEmail { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public bool IsApproved { get; set; } = false;
    
    // Foreign key
    public Guid BlogPostId { get; set; }
    
    // Navigation property
    public BlogPost BlogPost { get; set; } = null!;
}
```

**Business Rules:**
- Comments require approval before being publicly visible
- Must be associated with a blog post
- AuthorName, AuthorEmail, and Content are required

---

## Data Transfer Objects (DTOs)

### Author DTOs

#### AuthorDto (Response)
```csharp
public class AuthorDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Bio { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;     // Computed
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public int PostCount { get; set; }                       // Computed
}
```

#### CreateAuthorDto (Request)
```csharp
public class CreateAuthorDto
{
    public string FirstName { get; set; } = string.Empty;    // Required
    public string LastName { get; set; } = string.Empty;     // Required
    public string Email { get; set; } = string.Empty;        // Required, Unique
    public string Bio { get; set; } = string.Empty;          // Optional
}
```

#### UpdateAuthorDto (Request)
```csharp
public class UpdateAuthorDto
{
    public string FirstName { get; set; } = string.Empty;    // Required
    public string LastName { get; set; } = string.Empty;     // Required
    public string Bio { get; set; } = string.Empty;          // Optional
    // Note: Email cannot be updated
}
```

### BlogPost DTOs

#### BlogPostDto (Response)
```csharp
public class BlogPostDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public bool IsPublished { get; set; }
    public DateTime? PublishedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public Guid AuthorId { get; set; }
    public string AuthorName { get; set; } = string.Empty;   // Computed
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty; // Computed
    public int CommentCount { get; set; }                    // Computed
}
```

#### CreateBlogPostDto (Request)
```csharp
public class CreateBlogPostDto
{
    public string Title { get; set; } = string.Empty;        // Required
    public string Content { get; set; } = string.Empty;      // Required
    public string Summary { get; set; } = string.Empty;      // Required
    public Guid AuthorId { get; set; }                       // Required
    public Guid CategoryId { get; set; }                     // Required
    public bool IsPublished { get; set; } = false;           // Optional
}
```

#### UpdateBlogPostDto (Request)
```csharp
public class UpdateBlogPostDto
{
    public string Title { get; set; } = string.Empty;        // Required
    public string Content { get; set; } = string.Empty;      // Required
    public string Summary { get; set; } = string.Empty;      // Required
    public Guid CategoryId { get; set; }                     // Required
    public bool IsPublished { get; set; }                    // Optional
    // Note: AuthorId cannot be updated
}
```

### Category DTOs

#### CategoryDto (Response)
```csharp
public class CategoryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public int PostCount { get; set; }                       // Computed
}
```

#### CreateCategoryDto (Request)
```csharp
public class CreateCategoryDto
{
    public string Name { get; set; } = string.Empty;         // Required, Unique
    public string Description { get; set; } = string.Empty;  // Optional
}
```

#### UpdateCategoryDto (Request)
```csharp
public class UpdateCategoryDto
{
    public string Name { get; set; } = string.Empty;         // Required, Unique
    public string Description { get; set; } = string.Empty;  // Optional
}
```

### Comment DTOs

#### CommentDto (Response)
```csharp
public class CommentDto
{
    public Guid Id { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public string AuthorEmail { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public bool IsApproved { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid BlogPostId { get; set; }
    public string BlogPostTitle { get; set; } = string.Empty; // Computed
}
```

#### CreateCommentDto (Request)
```csharp
public class CreateCommentDto
{
    public string AuthorName { get; set; } = string.Empty;   // Required
    public string AuthorEmail { get; set; } = string.Empty;  // Required
    public string Content { get; set; } = string.Empty;      // Required
    public Guid BlogPostId { get; set; }                     // Required
}
```

#### UpdateCommentDto (Request)
```csharp
public class UpdateCommentDto
{
    public string AuthorName { get; set; } = string.Empty;   // Required
    public string AuthorEmail { get; set; } = string.Empty;  // Required
    public string Content { get; set; } = string.Empty;      // Required
    // Note: BlogPostId and IsApproved cannot be updated via this DTO
}
```

---

## CQRS Implementation

The application uses **CQRS (Command Query Responsibility Segregation)** pattern with **MediatR** library to separate read and write operations.

### Commands (Write Operations)

Commands represent operations that modify system state (Create, Update, Delete).

#### Author Commands

**CreateAuthorCommand**
```csharp
public record CreateAuthorCommand(
    string FirstName,
    string LastName,
    string Email,
    string? Bio = null
) : IRequest<AuthorDto>;
```

**UpdateAuthorCommand**
```csharp
public record UpdateAuthorCommand(
    Guid Id,
    string FirstName,
    string LastName,
    string? Bio = null
) : IRequest<AuthorDto>;
```

**DeleteAuthorCommand**
```csharp
public record DeleteAuthorCommand(Guid Id) : IRequest<bool>;
```

#### BlogPost Commands

**CreateBlogPostCommand**
```csharp
public record CreateBlogPostCommand(
    string Title,
    string Content,
    string Summary,
    Guid AuthorId,
    Guid CategoryId,
    bool IsPublished = false
) : IRequest<BlogPostDto>;
```

**PublishBlogPostCommand**
```csharp
public record PublishBlogPostCommand(Guid Id) : IRequest<BlogPostDto>;
```

**UnpublishBlogPostCommand**
```csharp
public record UnpublishBlogPostCommand(Guid Id) : IRequest<BlogPostDto>;
```

### Queries (Read Operations)

Queries represent operations that read data without modifying system state.

#### Author Queries

**GetAllAuthorsQuery**
```csharp
public record GetAllAuthorsQuery() : IRequest<IEnumerable<AuthorDto>>;
```

**GetAuthorByIdQuery**
```csharp
public record GetAuthorByIdQuery(Guid Id) : IRequest<AuthorDto?>;
```

**GetAuthorByEmailQuery**
```csharp
public record GetAuthorByEmailQuery(string Email) : IRequest<AuthorDto?>;
```

#### BlogPost Queries

**GetAllBlogPostsQuery**
```csharp
public record GetAllBlogPostsQuery() : IRequest<IEnumerable<BlogPostDto>>;
```

**GetPublishedBlogPostsQuery**
```csharp
public record GetPublishedBlogPostsQuery() : IRequest<IEnumerable<BlogPostDto>>;
```

**GetBlogPostsByCategoryQuery**
```csharp
public record GetBlogPostsByCategoryQuery(Guid CategoryId) : IRequest<IEnumerable<BlogPostDto>>;
```

### Command/Query Handlers

All handlers follow the same pattern implementing `IRequestHandler<TRequest, TResponse>`:

```csharp
public class CreateAuthorCommandHandler : IRequestHandler<CreateAuthorCommand, AuthorDto>
{
    private readonly IAuthorRepository _authorRepository;
    private readonly IMapper _mapper;

    public async Task<AuthorDto> Handle(CreateAuthorCommand request, CancellationToken cancellationToken)
    {
        // Business logic validation
        var existingAuthor = await _authorRepository.GetByEmailAsync(request.Email);
        if (existingAuthor != null)
        {
            throw new InvalidOperationException($"Author with email {request.Email} already exists.");
        }

        // Create and persist entity
        var author = new Author
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            Bio = request.Bio ?? string.Empty,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var createdAuthor = await _authorRepository.AddAsync(author);
        return _mapper.Map<AuthorDto>(createdAuthor);
    }
}
```

---

## Repository Pattern

### Base Repository Interface

```csharp
public interface IRepository<T> where T : BaseEntity
{
    Task<T?> GetByIdAsync(Guid id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<T> AddAsync(T entity);
    Task<T> UpdateAsync(T entity);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
}
```

### Specialized Repository Interfaces

#### IAuthorRepository
```csharp
public interface IAuthorRepository : IRepository<Author>
{
    Task<Author?> GetByEmailAsync(string email);
}
```

#### IBlogPostRepository
```csharp
public interface IBlogPostRepository : IRepository<BlogPost>
{
    Task<IEnumerable<BlogPost>> GetPublishedAsync();
    Task<IEnumerable<BlogPost>> GetByAuthorIdAsync(Guid authorId);
    Task<IEnumerable<BlogPost>> GetByCategoryIdAsync(Guid categoryId);
}
```

#### ICategoryRepository
```csharp
public interface ICategoryRepository : IRepository<Category>
{
    Task<Category?> GetByNameAsync(string name);
}
```

#### ICommentRepository
```csharp
public interface ICommentRepository : IRepository<Comment>
{
    Task<IEnumerable<Comment>> GetByBlogPostIdAsync(Guid blogPostId);
    Task<IEnumerable<Comment>> GetApprovedByBlogPostIdAsync(Guid blogPostId);
}
```

---

## REST API Endpoints

### Authors Controller (`/api/authors`)

#### Get All Authors
```http
GET /api/authors
```
**Response**: `200 OK` with `IEnumerable<AuthorDto>`

#### Get Author by ID
```http
GET /api/authors/{id}
```
**Parameters:**
- `id` (Guid): Author ID

**Responses:**
- `200 OK`: Returns `AuthorDto`
- `404 Not Found`: Author not found

#### Get Author by Email
```http
GET /api/authors/email/{email}
```
**Parameters:**
- `email` (string): Author email address

**Responses:**
- `200 OK`: Returns `AuthorDto`
- `404 Not Found`: Author not found

#### Create Author
```http
POST /api/authors
Content-Type: application/json

{
    "firstName": "John",
    "lastName": "Doe",
    "email": "john.doe@example.com",
    "bio": "Software Engineer and Tech Writer"
}
```
**Request Body**: `CreateAuthorDto`

**Responses:**
- `201 Created`: Returns created `AuthorDto` with `Location` header
- `400 Bad Request`: Validation errors or email already exists

#### Update Author
```http
PUT /api/authors/{id}
Content-Type: application/json

{
    "firstName": "John",
    "lastName": "Smith",
    "bio": "Updated bio"
}
```
**Parameters:**
- `id` (Guid): Author ID

**Request Body**: `UpdateAuthorDto`

**Responses:**
- `200 OK`: Returns updated `AuthorDto`
- `400 Bad Request`: Validation errors
- `404 Not Found`: Author not found

#### Delete Author
```http
DELETE /api/authors/{id}
```
**Parameters:**
- `id` (Guid): Author ID

**Responses:**
- `204 No Content`: Author deleted successfully
- `400 Bad Request`: Author has associated blog posts
- `404 Not Found`: Author not found

### BlogPosts Controller (`/api/blogposts`)

#### Get All Blog Posts
```http
GET /api/blogposts
```
**Response**: `200 OK` with `IEnumerable<BlogPostDto>`

#### Get Published Blog Posts
```http
GET /api/blogposts/published
```
**Response**: `200 OK` with `IEnumerable<BlogPostDto>`

#### Get Blog Post by ID
```http
GET /api/blogposts/{id}
```
**Parameters:**
- `id` (Guid): Blog post ID

**Responses:**
- `200 OK`: Returns `BlogPostDto`
- `404 Not Found`: Blog post not found

#### Get Blog Posts by Author
```http
GET /api/blogposts/author/{authorId}
```
**Parameters:**
- `authorId` (Guid): Author ID

**Response**: `200 OK` with `IEnumerable<BlogPostDto>`

#### Get Blog Posts by Category
```http
GET /api/blogposts/category/{categoryId}
```
**Parameters:**
- `categoryId` (Guid): Category ID

**Response**: `200 OK` with `IEnumerable<BlogPostDto>`

#### Create Blog Post
```http
POST /api/blogposts
Content-Type: application/json

{
    "title": "Getting Started with Clean Architecture",
    "content": "Full blog post content here...",
    "summary": "A comprehensive guide to implementing clean architecture",
    "authorId": "123e4567-e89b-12d3-a456-426614174000",
    "categoryId": "456e7890-e89b-12d3-a456-426614174000",
    "isPublished": false
}
```
**Request Body**: `CreateBlogPostDto`

**Responses:**
- `201 Created`: Returns created `BlogPostDto` with `Location` header
- `400 Bad Request`: Validation errors or invalid author/category IDs

#### Update Blog Post
```http
PUT /api/blogposts/{id}
Content-Type: application/json

{
    "title": "Updated Title",
    "content": "Updated content...",
    "summary": "Updated summary",
    "categoryId": "456e7890-e89b-12d3-a456-426614174000",
    "isPublished": true
}
```
**Parameters:**
- `id` (Guid): Blog post ID

**Request Body**: `UpdateBlogPostDto`

**Responses:**
- `200 OK`: Returns updated `BlogPostDto`
- `400 Bad Request`: Validation errors
- `404 Not Found`: Blog post not found

#### Publish Blog Post
```http
POST /api/blogposts/{id}/publish
```
**Parameters:**
- `id` (Guid): Blog post ID

**Responses:**
- `200 OK`: Returns updated `BlogPostDto`
- `404 Not Found`: Blog post not found

#### Unpublish Blog Post
```http
POST /api/blogposts/{id}/unpublish
```
**Parameters:**
- `id` (Guid): Blog post ID

**Responses:**
- `200 OK`: Returns updated `BlogPostDto`
- `404 Not Found`: Blog post not found

#### Delete Blog Post
```http
DELETE /api/blogposts/{id}
```
**Parameters:**
- `id` (Guid): Blog post ID

**Responses:**
- `204 No Content`: Blog post deleted successfully
- `404 Not Found`: Blog post not found

### Categories Controller (`/api/categories`)

#### Get All Categories
```http
GET /api/categories
```
**Response**: `200 OK` with `IEnumerable<CategoryDto>`

#### Get Category by ID
```http
GET /api/categories/{id}
```
**Parameters:**
- `id` (Guid): Category ID

**Responses:**
- `200 OK`: Returns `CategoryDto`
- `404 Not Found`: Category not found

#### Get Category by Name
```http
GET /api/categories/name/{name}
```
**Parameters:**
- `name` (string): Category name

**Responses:**
- `200 OK`: Returns `CategoryDto`
- `404 Not Found`: Category not found

#### Create Category
```http
POST /api/categories
Content-Type: application/json

{
    "name": "Technology",
    "description": "Posts about technology and software development"
}
```
**Request Body**: `CreateCategoryDto`

**Responses:**
- `201 Created`: Returns created `CategoryDto` with `Location` header
- `400 Bad Request`: Validation errors or name already exists

#### Update Category
```http
PUT /api/categories/{id}
Content-Type: application/json

{
    "name": "Tech & Innovation",
    "description": "Updated description"
}
```
**Parameters:**
- `id` (Guid): Category ID

**Request Body**: `UpdateCategoryDto`

**Responses:**
- `200 OK`: Returns updated `CategoryDto`
- `400 Bad Request`: Validation errors or name conflict
- `404 Not Found`: Category not found

#### Delete Category
```http
DELETE /api/categories/{id}
```
**Parameters:**
- `id` (Guid): Category ID

**Responses:**
- `204 No Content`: Category deleted successfully
- `400 Bad Request`: Category has associated blog posts
- `404 Not Found`: Category not found

### Comments Controller (`/api/comments`)

#### Get All Comments
```http
GET /api/comments
```
**Response**: `200 OK` with `IEnumerable<CommentDto>`

#### Get Comment by ID
```http
GET /api/comments/{id}
```
**Parameters:**
- `id` (Guid): Comment ID

**Responses:**
- `200 OK`: Returns `CommentDto`
- `404 Not Found`: Comment not found

#### Get Comments for Blog Post
```http
GET /api/comments/post/{blogPostId}
```
**Parameters:**
- `blogPostId` (Guid): Blog post ID

**Response**: `200 OK` with `IEnumerable<CommentDto>`

#### Get Approved Comments for Blog Post
```http
GET /api/comments/post/{blogPostId}/approved
```
**Parameters:**
- `blogPostId` (Guid): Blog post ID

**Response**: `200 OK` with `IEnumerable<CommentDto>` (approved only)

#### Create Comment
```http
POST /api/comments
Content-Type: application/json

{
    "authorName": "Jane Smith",
    "authorEmail": "jane@example.com",
    "content": "Great article! Very informative.",
    "blogPostId": "789e1234-e89b-12d3-a456-426614174000"
}
```
**Request Body**: `CreateCommentDto`

**Responses:**
- `201 Created`: Returns created `CommentDto` with `Location` header
- `400 Bad Request`: Validation errors or invalid blog post ID

#### Update Comment
```http
PUT /api/comments/{id}
Content-Type: application/json

{
    "authorName": "Jane Smith",
    "authorEmail": "jane.smith@example.com",
    "content": "Updated comment content"
}
```
**Parameters:**
- `id` (Guid): Comment ID

**Request Body**: `UpdateCommentDto`

**Responses:**
- `200 OK`: Returns updated `CommentDto`
- `400 Bad Request`: Validation errors
- `404 Not Found`: Comment not found

#### Approve Comment
```http
POST /api/comments/{id}/approve
```
**Parameters:**
- `id` (Guid): Comment ID

**Responses:**
- `200 OK`: Returns updated `CommentDto`
- `404 Not Found`: Comment not found

#### Reject Comment
```http
POST /api/comments/{id}/reject
```
**Parameters:**
- `id` (Guid): Comment ID

**Responses:**
- `200 OK`: Returns updated `CommentDto`
- `404 Not Found`: Comment not found

#### Delete Comment
```http
DELETE /api/comments/{id}
```
**Parameters:**
- `id` (Guid): Comment ID

**Responses:**
- `204 No Content`: Comment deleted successfully
- `404 Not Found`: Comment not found

---

## Configuration and Middleware

### Application Configuration (Program.cs)

```csharp
var builder = WebApplication.CreateBuilder(args);

// Database Configuration
builder.Services.AddDbContext<BlogDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// AutoMapper Registration
builder.Services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfile>());

// Repository Registration
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IBlogPostRepository, BlogPostRepository>();
builder.Services.AddScoped<IAuthorRepository, AuthorRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ICommentRepository, CommentRepository>();

// MediatR Registration for CQRS
builder.Services.AddMediatR(cfg => {
    cfg.RegisterServicesFromAssembly(typeof(CreateAuthorCommand).Assembly);
});

// API Configuration
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Swagger Configuration
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "BlogSite API",
        Version = "v1",
        Description = "A comprehensive blog management API built with ASP.NET Core 8.0"
    });
});

// CORS Configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("BlogSitePolicy", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});
```

### Middleware Pipeline

```csharp
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "BlogSite API v1");
        c.RoutePrefix = string.Empty; // Serve Swagger UI at root
    });
}

app.UseHttpsRedirection();
app.UseCors("BlogSitePolicy");
app.UseAuthorization();
app.MapControllers();

// Database Initialization
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<BlogDbContext>();
    dbContext.Database.EnsureCreated();
}
```

### Connection Strings (appsettings.json)

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=BlogSite.db"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.EntityFrameworkCore": "Information"
    }
  }
}
```

---

## Usage Examples

### Complete Workflow Example

#### 1. Create an Author
```bash
curl -X POST "https://localhost:5001/api/authors" \
  -H "Content-Type: application/json" \
  -d '{
    "firstName": "John",
    "lastName": "Doe",
    "email": "john.doe@example.com",
    "bio": "Software engineer and technical writer"
  }'
```

**Response:**
```json
{
  "id": "123e4567-e89b-12d3-a456-426614174000",
  "firstName": "John",
  "lastName": "Doe",
  "email": "john.doe@example.com",
  "bio": "Software engineer and technical writer",
  "fullName": "John Doe",
  "createdAt": "2024-01-15T10:30:00Z",
  "updatedAt": "2024-01-15T10:30:00Z",
  "postCount": 0
}
```

#### 2. Create a Category
```bash
curl -X POST "https://localhost:5001/api/categories" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Technology",
    "description": "Posts about software development and technology trends"
  }'
```

#### 3. Create a Blog Post
```bash
curl -X POST "https://localhost:5001/api/blogposts" \
  -H "Content-Type: application/json" \
  -d '{
    "title": "Getting Started with Clean Architecture",
    "content": "Clean Architecture is a software design philosophy...",
    "summary": "Learn the fundamentals of clean architecture",
    "authorId": "123e4567-e89b-12d3-a456-426614174000",
    "categoryId": "456e7890-e89b-12d3-a456-426614174000",
    "isPublished": false
  }'
```

#### 4. Publish the Blog Post
```bash
curl -X POST "https://localhost:5001/api/blogposts/789e1234-e89b-12d3-a456-426614174000/publish"
```

#### 5. Add a Comment
```bash
curl -X POST "https://localhost:5001/api/comments" \
  -H "Content-Type: application/json" \
  -d '{
    "authorName": "Jane Smith",
    "authorEmail": "jane@example.com",
    "content": "Excellent article! Very helpful.",
    "blogPostId": "789e1234-e89b-12d3-a456-426614174000"
  }'
```

#### 6. Approve the Comment
```bash
curl -X POST "https://localhost:5001/api/comments/abc12345-e89b-12d3-a456-426614174000/approve"
```

### C# Client Example

```csharp
public class BlogSiteClient
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;

    public BlogSiteClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    public async Task<AuthorDto> CreateAuthorAsync(CreateAuthorDto createDto)
    {
        var json = JsonSerializer.Serialize(createDto, _jsonOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        
        var response = await _httpClient.PostAsync("/api/authors", content);
        response.EnsureSuccessStatusCode();
        
        var responseJson = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<AuthorDto>(responseJson, _jsonOptions)!;
    }

    public async Task<IEnumerable<AuthorDto>> GetAllAuthorsAsync()
    {
        var response = await _httpClient.GetAsync("/api/authors");
        response.EnsureSuccessStatusCode();
        
        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<IEnumerable<AuthorDto>>(json, _jsonOptions)!;
    }

    public async Task<BlogPostDto> PublishBlogPostAsync(Guid blogPostId)
    {
        var response = await _httpClient.PostAsync($"/api/blogposts/{blogPostId}/publish", null);
        response.EnsureSuccessStatusCode();
        
        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<BlogPostDto>(json, _jsonOptions)!;
    }
}
```

### JavaScript/TypeScript Example

```typescript
interface AuthorDto {
    id: string;
    firstName: string;
    lastName: string;
    email: string;
    bio: string;
    fullName: string;
    createdAt: string;
    updatedAt: string;
    postCount: number;
}

interface CreateAuthorDto {
    firstName: string;
    lastName: string;
    email: string;
    bio?: string;
}

class BlogSiteApiClient {
    constructor(private baseUrl: string) {}

    async createAuthor(createDto: CreateAuthorDto): Promise<AuthorDto> {
        const response = await fetch(`${this.baseUrl}/api/authors`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(createDto),
        });

        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }

        return await response.json();
    }

    async getAllAuthors(): Promise<AuthorDto[]> {
        const response = await fetch(`${this.baseUrl}/api/authors`);
        
        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }

        return await response.json();
    }

    async publishBlogPost(blogPostId: string): Promise<BlogPostDto> {
        const response = await fetch(`${this.baseUrl}/api/blogposts/${blogPostId}/publish`, {
            method: 'POST',
        });

        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }

        return await response.json();
    }
}

// Usage
const client = new BlogSiteApiClient('https://localhost:5001');

// Create an author
const newAuthor = await client.createAuthor({
    firstName: 'John',
    lastName: 'Doe',
    email: 'john.doe@example.com',
    bio: 'Software engineer'
});

console.log('Created author:', newAuthor);
```

---

## Error Handling

### Standard HTTP Status Codes

| Status Code | Description | Usage |
|-------------|-------------|-------|
| `200 OK` | Success | GET, PUT operations |
| `201 Created` | Resource created | POST operations |
| `204 No Content` | Success, no content | DELETE operations |
| `400 Bad Request` | Validation errors, business rule violations | Invalid input data |
| `404 Not Found` | Resource not found | Invalid ID parameters |
| `500 Internal Server Error` | Unexpected server errors | System exceptions |

### Error Response Format

All error responses follow a consistent format:

```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "Bad Request",
  "status": 400,
  "errors": {
    "FirstName": ["The FirstName field is required."],
    "Email": ["The Email field is required.", "Invalid email format."]
  },
  "traceId": "00-1234567890abcdef-1234567890abcdef-01"
}
```

### Common Error Scenarios

#### Validation Errors (400 Bad Request)
```json
{
  "status": 400,
  "title": "Validation failed",
  "errors": {
    "Email": ["Email is required"],
    "FirstName": ["FirstName cannot be empty"]
  }
}
```

#### Business Rule Violations (400 Bad Request)
```json
{
  "status": 400,
  "title": "Business rule violation",
  "detail": "Author with email john.doe@example.com already exists."
}
```

#### Resource Not Found (404 Not Found)
```json
{
  "status": 404,
  "title": "Resource not found",
  "detail": "Author with ID 123e4567-e89b-12d3-a456-426614174000 not found"
}
```

#### Constraint Violations (400 Bad Request)
```json
{
  "status": 400,
  "title": "Constraint violation",
  "detail": "Cannot delete author because they have associated blog posts"
}
```

### Exception Handling in Controllers

All controllers implement consistent exception handling:

```csharp
[HttpPost]
public async Task<ActionResult<AuthorDto>> CreateAuthor([FromBody] CreateAuthorDto createDto)
{
    try
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var command = new CreateAuthorCommand(/*...*/);
        var createdAuthor = await _mediator.Send(command);
        
        return CreatedAtAction(nameof(GetAuthor), new { id = createdAuthor.Id }, createdAuthor);
    }
    catch (InvalidOperationException ex)
    {
        _logger.LogWarning(ex, "Invalid operation while creating author");
        return BadRequest(ex.Message);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error occurred while creating author");
        return StatusCode(500, "An error occurred while processing your request");
    }
}
```

---

## Getting Started

### Prerequisites
- .NET 8.0 SDK
- SQLite (included with .NET)
- IDE (Visual Studio, VS Code, or Rider)

### Running the Application

1. **Clone and build the solution:**
   ```bash
   git clone <repository-url>
   cd BlogSite
   dotnet build
   ```

2. **Run the API:**
   ```bash
   cd src/BlogSite.API
   dotnet run
   ```

3. **Access the API:**
   - API Base URL: `https://localhost:5001`
   - Swagger UI: `https://localhost:5001` (served at root)
   - Health Check: `https://localhost:5001/api/authors` (should return empty array initially)

### Sample Data

The application automatically creates and seeds the database with sample data on first run:
- Sample author: "John Doe"
- Sample category: "Technology"
- Sample blog post: "Welcome to BlogSite"

### Development Environment

For development, the application uses:
- SQLite database (BlogSite.db)
- Console logging with Entity Framework query logging
- CORS enabled for all origins
- Swagger UI served at application root
- Automatic database creation and migration

---

## Support and Contributing

### API Documentation
- **Swagger/OpenAPI**: Available at the application root when running
- **Postman Collection**: Import the OpenAPI specification
- **Interactive Testing**: Use Swagger UI for live API testing

### Logging
All operations are logged with appropriate log levels:
- **Information**: Successful operations
- **Warning**: Business rule violations, validation errors
- **Error**: System exceptions and unexpected errors

### Future Enhancements
- Authentication and authorization
- Caching layer (Redis)
- Search functionality
- File upload for images
- Email notifications
- Rate limiting
- Unit and integration tests
- Performance monitoring

---

This documentation provides comprehensive coverage of all public APIs, functions, and components in the BlogSite project. Each section includes detailed examples and usage instructions to help developers understand and integrate with the API effectively.