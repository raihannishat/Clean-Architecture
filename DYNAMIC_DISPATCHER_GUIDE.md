# Dynamic Dispatcher System Guide

## Overview

The dispatcher system has been completely refactored to be fully dynamic. You no longer need to manually register operations for each new entity. The system automatically discovers and registers all commands and queries using reflection and naming conventions.

## Key Benefits

✅ **No Manual Registration**: New entities are automatically discovered  
✅ **Convention-Based**: Follows consistent naming patterns  
✅ **Future-Proof**: New entities require zero configuration changes  
✅ **Self-Documenting**: Operation descriptions are generated automatically  
✅ **Type-Safe**: Full compile-time type checking maintained  

## How It Works

### 1. Automatic Discovery

The system automatically discovers operations by:

- Scanning assemblies for types implementing `IRequest` or `IRequest<T>`
- Parsing type names using conventions: `{Action}{Entity}{OperationType}`
- Using `EntityDiscoveryService` to identify entity types dynamically
- Caching results for optimal performance

### 2. Naming Conventions

Your commands and queries must follow these conventions:

#### Commands
```csharp
// Pattern: {Action}{Entity}Command
public record CreateAuthorCommand(...) : IRequest<AuthorDto>;
public record UpdateBlogPostCommand(...) : IRequest<BlogPostDto>;
public record DeleteCategoryCommand(...) : IRequest<bool>;
public record ActivateUserCommand(...) : IRequest<UserDto>;
```

#### Queries
```csharp
// Pattern: {Action}{Entity}Query or Get{Something}{Entity}Query
public record GetAllAuthorsQuery() : IRequest<IEnumerable<AuthorDto>>;
public record GetAuthorByIdQuery(int Id) : IRequest<AuthorDto>;
public record GetPublishedBlogPostsQuery() : IRequest<IEnumerable<BlogPostDto>>;
public record SearchUsersQuery(string Term) : IRequest<IEnumerable<UserDto>>;
```

### 3. Directory Structure

Organize your commands and queries by entity:

```
src/BlogSite.Application/
├── Commands/
│   ├── Authors/
│   │   ├── CreateAuthorCommand.cs
│   │   ├── UpdateAuthorCommand.cs
│   │   └── DeleteAuthorCommand.cs
│   ├── BlogPosts/
│   │   ├── CreateBlogPostCommand.cs
│   │   └── PublishBlogPostCommand.cs
│   └── NewEntity/           ← Just add your new entity here!
│       ├── CreateNewEntityCommand.cs
│       └── UpdateNewEntityCommand.cs
└── Queries/
    ├── Authors/
    │   ├── GetAllAuthorsQuery.cs
    │   └── GetAuthorByIdQuery.cs
    ├── BlogPosts/
    │   └── GetPublishedBlogPostsQuery.cs
    └── NewEntity/              ← And here!
        ├── GetAllNewEntitiesQuery.cs
        └── GetNewEntityByIdQuery.cs
```

## Adding a New Entity

To add a new entity (e.g., `Product`), simply create your commands and queries following the naming convention:

### Step 1: Create Commands

```csharp
// Commands/Products/CreateProductCommand.cs
public record CreateProductCommand(
    string Name,
    decimal Price,
    string Description
) : IRequest<ProductDto>;

// Commands/Products/UpdateProductCommand.cs
public record UpdateProductCommand(
    int Id,
    string Name,
    decimal Price,
    string Description
) : IRequest<ProductDto>;
```

### Step 2: Create Queries

```csharp
// Queries/Products/GetAllProductsQuery.cs
public record GetAllProductsQuery() : IRequest<IEnumerable<ProductDto>>;

// Queries/Products/GetProductByIdQuery.cs
public record GetProductByIdQuery(int Id) : IRequest<ProductDto>;
```

### Step 3: That's It!

No registration needed! The system will automatically:

- Discover your new `Product` entity
- Parse the operation types (`Command` vs `Query`)
- Extract actions (`Create`, `Update`, `GetAll`, `GetById`)
- Generate appropriate descriptions
- Make them available through the dispatcher

## Supported Action Types

The system recognizes these common actions and generates appropriate descriptions:

### Commands
- `Create` → "Creates a new {entity}"
- `Update` → "Updates an existing {entity}"
- `Delete` → "Deletes a {entity}"
- `Publish` → "Publishes a {entity}"
- `Archive` → "Archives a {entity}"
- `Activate` → "Activates a {entity}"
- `Deactivate` → "Deactivates a {entity}"
- `Approve` → "Approves a {entity}"
- `Reject` → "Rejects a {entity}"

### Queries
- `GetAll` → "Gets all {entity}s"
- `GetById` → "Gets a {entity} by ID"
- `GetByEmail` → "Gets a {entity} by email"
- `GetByName` → "Gets a {entity} by name"
- `GetPublished` → "Gets published {entity}s"
- `Search` → "Searches {entity}s"

## Dynamic API Features

### Get Available Entities

```csharp
var entities = registry.GetAvailableEntityTypes();
// Returns: ["Author", "BlogPost", "Category", "Product"]
```

### Get Actions for Entity

```csharp
var actions = registry.GetAvailableActions("Product");
// Returns: ["Create", "Update", "GetAll", "GetById"]
```

### Get Operations by Entity

```csharp
var operations = registry.GetOperationsByEntity();
// Returns grouped operations for easy API generation
```

### Get Operation Summaries

```csharp
var summaries = registry.GetOperationSummaries();
// Returns detailed information about all operations
```

## API Usage

Use the dynamic dispatcher in your controllers:

```csharp
[ApiController]
[Route("api/dynamic")]
public class DynamicController : ControllerBase
{
    private readonly IDispatcher _dispatcher;

    public DynamicController(IDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }

    [HttpPost("{entityType}/{action}")]
    public async Task<IActionResult> ExecuteCommand(
        string entityType, 
        string action, 
        [FromBody] JsonElement payload)
    {
        var request = new DispatchRequest(entityType, action, payload);
        var result = await _dispatcher.DispatchAsync(request);
        
        return result.Success ? Ok(result.Data) : BadRequest(result.ErrorMessage);
    }
}
```

## Best Practices

1. **Consistent Naming**: Always follow the `{Action}{Entity}{Type}` convention
2. **Entity Folders**: Organize commands/queries in entity-specific folders
3. **Clear Actions**: Use descriptive action names that clearly indicate intent
4. **Return Types**: Always specify appropriate return types for queries
5. **Documentation**: Add XML documentation to your commands/queries

## Troubleshooting

### Operation Not Found

If an operation isn't discovered:

1. Check the naming convention matches exactly
2. Ensure the type implements `IRequest` or `IRequest<T>`
3. Verify the type is not abstract or generic
4. Check that the assembly is loaded

### Entity Not Recognized

If an entity isn't recognized:

1. Ensure at least one command/query exists for the entity
2. Check the naming follows the convention
3. Verify the entity name is consistent across all operations

### Performance

The system caches discovered operations for optimal performance. If you add new operations at runtime, they'll be discovered on first use.

## Migration from Old System

The old hardcoded registration methods have been removed:

```csharp
// ❌ OLD - Manual registration (removed)
private static void RegisterAuthorOperations(IRequestTypeRegistry registry)
{
    registry.RegisterOperation("Command", "Author", "Create", typeof(CreateAuthorCommand), typeof(AuthorDto));
    // ... more registrations
}

// ✅ NEW - Automatic discovery
public static IServiceProvider RegisterAllOperations(this IServiceProvider serviceProvider)
{
    // Everything is discovered automatically!
    var registry = serviceProvider.GetRequiredService<IRequestTypeRegistry>();
    _ = registry.GetAllOperations().ToList(); // Optional: warm up cache
    return serviceProvider;
}
```

The new system is backward compatible - existing operations will continue to work without changes.