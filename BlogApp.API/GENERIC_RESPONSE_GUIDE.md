# Generic Response Structure Guide

This guide explains the new generic response structure implemented across all commands and queries in the BlogApp API.

## Overview

All API operations now return a consistent `BaseResponse<T>` structure that provides:
- Standardized success/failure indicators
- Consistent error handling
- HTTP status codes
- Descriptive messages
- Multiple error support

## BaseResponse Structure

### Generic Response
```csharp
public class BaseResponse<T>
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
    public List<string> Errors { get; set; } = new();
    public int StatusCode { get; set; } = 200;
}
```

### Non-Generic Response
```csharp
public class BaseResponse : BaseResponse<object>
{
    // For operations that don't return data
}
```

## Response Examples

### Success Response
```json
{
  "isSuccess": true,
  "message": "Blog post created successfully",
  "data": {
    "id": 1,
    "title": "My Blog Post",
    "content": "Post content...",
    "slug": "my-blog-post",
    "createdAt": "2024-01-15T10:30:00Z"
  },
  "errors": [],
  "statusCode": 200
}
```

### Error Response
```json
{
  "isSuccess": false,
  "message": "Validation failed",
  "data": null,
  "errors": [
    "Title is required",
    "Content must be at least 50 characters"
  ],
  "statusCode": 400
}
```

### Not Found Response
```json
{
  "isSuccess": false,
  "message": "Blog post not found",
  "data": null,
  "errors": [],
  "statusCode": 404
}
```

## Static Factory Methods

### Success Responses
```csharp
// With data
return BaseResponse<BlogPost>.Success(blogPost, "Blog post created successfully");

// Without data
return BaseResponse.Success("Operation completed successfully");
```

### Error Responses
```csharp
// Single error
return BaseResponse<BlogPost>.Failure("Failed to create blog post", 500);

// Multiple errors
return BaseResponse<BlogPost>.Failure(
    new List<string> { "Title is required", "Content is required" },
    "Validation failed",
    400
);
```

### HTTP Status Responses
```csharp
// 404 Not Found
return BaseResponse<BlogPost>.NotFound("Blog post not found");

// 401 Unauthorized
return BaseResponse<BlogPost>.Unauthorized("Invalid credentials");

// 403 Forbidden
return BaseResponse<BlogPost>.Forbidden("Access denied");

// 400 Validation Error
return BaseResponse<BlogPost>.ValidationError(
    new List<string> { "Title is required" },
    "Validation failed"
);
```

## Implementation in Commands

### Before (Direct Return)
```csharp
public class CreateBlogPostCommand : ICommand<BlogPost>
{
    // Properties...
}

public class CreateBlogPostCommandHandler : ICommandHandler<CreateBlogPostCommand, BlogPost>
{
    public async Task<BlogPost> HandleAsync(CreateBlogPostCommand command, CancellationToken cancellationToken = default)
    {
        var blogPost = new BlogPost { /* ... */ };
        await _unitOfWork.BlogPosts.AddAsync(blogPost);
        await _unitOfWork.SaveChangesAsync();
        return blogPost; // Direct return
    }
}
```

### After (BaseResponse)
```csharp
public class CreateBlogPostCommand : ICommand<BaseResponse<BlogPost>>
{
    // Properties...
}

public class CreateBlogPostCommandHandler : ICommandHandler<CreateBlogPostCommand, BaseResponse<BlogPost>>
{
    public async Task<BaseResponse<BlogPost>> HandleAsync(CreateBlogPostCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            // Validation
            var category = await _unitOfWork.Categories.GetByIdAsync(command.CategoryId);
            if (category == null)
            {
                return BaseResponse<BlogPost>.NotFound("Category not found");
            }

            // Business logic
            var blogPost = new BlogPost { /* ... */ };
            await _unitOfWork.BlogPosts.AddAsync(blogPost);
            await _unitOfWork.SaveChangesAsync();

            return BaseResponse<BlogPost>.Success(blogPost, "Blog post created successfully");
        }
        catch (Exception ex)
        {
            return BaseResponse<BlogPost>.Failure($"Failed to create blog post: {ex.Message}", 500);
        }
    }
}
```

## Implementation in Queries

### Before (Direct Return)
```csharp
public class GetBlogPostsQuery : IQuery<List<BlogPost>>
{
    // Properties...
}

public class GetBlogPostsQueryHandler : IQueryHandler<GetBlogPostsQuery, List<BlogPost>>
{
    public async Task<List<BlogPost>> HandleAsync(GetBlogPostsQuery query, CancellationToken cancellationToken = default)
    {
        var blogPosts = await _unitOfWork.BlogPosts.Query()
            .Where(bp => bp.IsPublished)
            .ToListAsync(cancellationToken);
        return blogPosts; // Direct return
    }
}
```

### After (BaseResponse)
```csharp
public class GetBlogPostsQuery : IQuery<BaseResponse<List<BlogPost>>>
{
    // Properties...
}

public class GetBlogPostsQueryHandler : IQueryHandler<GetBlogPostsQuery, BaseResponse<List<BlogPost>>>
{
    public async Task<BaseResponse<List<BlogPost>>> HandleAsync(GetBlogPostsQuery query, CancellationToken cancellationToken = default)
    {
        try
        {
            var blogPosts = await _unitOfWork.BlogPosts.Query()
                .Where(bp => bp.IsPublished)
                .ToListAsync(cancellationToken);

            var message = blogPosts.Count > 0 
                ? $"Retrieved {blogPosts.Count} blog posts" 
                : "No blog posts found";

            return BaseResponse<List<BlogPost>>.Success(blogPosts, message);
        }
        catch (Exception ex)
        {
            return BaseResponse<List<BlogPost>>.Failure($"Failed to retrieve blog posts: {ex.Message}", 500);
        }
    }
}
```

## Dispatcher Integration

The dispatcher endpoint automatically handles BaseResponse objects:

```csharp
// In DispatcherEndpoint.HandleAsync
if (result is BaseResponse baseResponse)
{
    await SendAsync(new DispatcherResponse
    {
        IsSuccess = baseResponse.IsSuccess,
        Message = baseResponse.Message,
        Data = baseResponse.Data,
        Errors = baseResponse.Errors,
        StatusCode = baseResponse.StatusCode
    }, cancellation: ct);
}
```

## Angular Integration

The Angular dispatcher service has been updated to handle the new response structure:

```typescript
// In DispatcherService
map((response: DispatcherResponse) => {
  if (response.isSuccess) {
    return response.data as T;
  } else {
    const errorMessage = response.errors && response.errors.length > 0 
      ? response.errors.join(', ') 
      : response.message || 'Unknown error occurred';
    throw new Error(errorMessage);
  }
})
```

## Benefits

1. **Consistency**: All operations return the same response structure
2. **Error Handling**: Standardized error messages and codes
3. **Validation**: Built-in support for multiple validation errors
4. **Status Codes**: Proper HTTP status codes for different scenarios
5. **Type Safety**: Generic responses maintain type safety
6. **Debugging**: Better error information for debugging
7. **Client Integration**: Easier integration with frontend applications

## Migration Guide

### Step 1: Update Interface
```csharp
// Change from
public class MyCommand : ICommand<MyData>

// To
public class MyCommand : ICommand<BaseResponse<MyData>>
```

### Step 2: Update Handler
```csharp
// Change from
public class MyCommandHandler : ICommandHandler<MyCommand, MyData>

// To
public class MyCommandHandler : ICommandHandler<MyCommand, BaseResponse<MyData>>
```

### Step 3: Update Handler Method
```csharp
// Change from
public async Task<MyData> HandleAsync(MyCommand command, CancellationToken cancellationToken = default)
{
    // Your logic
    return result;
}

// To
public async Task<BaseResponse<MyData>> HandleAsync(MyCommand command, CancellationToken cancellationToken = default)
{
    try
    {
        // Your logic
        return BaseResponse<MyData>.Success(result, "Operation successful");
    }
    catch (Exception ex)
    {
        return BaseResponse<MyData>.Failure($"Operation failed: {ex.Message}", 500);
    }
}
```

## Best Practices

1. **Always use try-catch**: Wrap handler logic in try-catch blocks
2. **Validate inputs**: Check for null references and invalid data
3. **Use appropriate status codes**: Return correct HTTP status codes
4. **Provide meaningful messages**: Give clear, user-friendly error messages
5. **Handle multiple errors**: Use the Errors collection for validation errors
6. **Be consistent**: Use the same patterns across all handlers

## Status Code Guidelines

- **200**: Success
- **201**: Created (for POST operations)
- **400**: Bad Request (validation errors)
- **401**: Unauthorized (authentication required)
- **403**: Forbidden (insufficient permissions)
- **404**: Not Found (resource doesn't exist)
- **409**: Conflict (resource already exists)
- **500**: Internal Server Error (unexpected errors) 