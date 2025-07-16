# Class Name Based Dispatcher Usage Guide

> **Note:** All API usage, dispatcher pattern, request/response format, and operation mapping documentation are now located in the `Docs/` directory at the project root. See also:
> - `Docs/API_TESTING_GUIDE.md`
> - `Docs/DISPATCHER_USAGE_EXAMPLES.md`
> - `Docs/GENERIC_RESPONSE_GUIDE.md`
> - `Docs/BASERESPONSE_MIGRATION_SUMMARY.md`

The API uses the **exact class name** for the dynamic dispatcher, making operation names consistent with your C# class names. All operations return a consistent `BaseResponse<T>` structure.

## How It Works

The dispatcher automatically maps operation names to your command/query class names exactly as they are defined in your code.

## Available Operations

### Authentication Operations

#### Login
```json
{
  "operation": "Login",
  "data": "{\"email\":\"user@example.com\",\"password\":\"password123\"}"
}
```

#### Register
```json
{
  "operation": "Register",
  "data": "{\"email\":\"newuser@example.com\",\"password\":\"password123\",\"firstName\":\"John\",\"lastName\":\"Doe\"}"
}
```

### Blog Operations

#### Get Blog Posts
```json
{
  "operation": "GetBlogPosts",
  "data": "{\"includeUnpublished\":false,\"page\":1,\"pageSize\":10}"
}
```

#### Get Blog Post by Slug
```json
{
  "operation": "GetBlogPostBySlug",
  "data": "{\"slug\":\"my-blog-post\"}"
}
```

#### Create Blog Post
```json
{
  "operation": "CreateBlogPost",
  "data": "{\"title\":\"My New Post\",\"content\":\"Post content...\",\"categoryId\":1,\"tagIds\":[1,2]}"
}
```

#### Get Categories
```json
{
  "operation": "GetCategories",
  "data": "{}"
}
```

#### Get Tags
```json
{
  "operation": "GetTags",
  "data": "{}"
}
```

#### Search Posts
```json
{
  "operation": "SearchPosts",
  "data": "{\"searchTerm\":\"keyword\",\"categoryId\":1,\"tagIds\":[1,2],\"page\":1,\"pageSize\":10}"
}
```

### Comment Operations

#### Get Comments
```json
{
  "operation": "GetComments",
  "data": "{\"postId\":1}"
}
```

#### Create Comment
```json
{
  "operation": "CreateComment",
  "data": "{\"content\":\"Great post!\",\"blogPostId\":1,\"parentCommentId\":null}"
}
```

## Operation Name Mapping

| Class Name | Operation Name |
|------------|----------------|
| `LoginCommand` | `LoginCommand` |
| `RegisterCommand` | `RegisterCommand` |
| `GetBlogPostsQuery` | `GetBlogPostsQuery` |
| `GetBlogPostBySlugQuery` | `GetBlogPostBySlugQuery` |
| `CreateBlogPostCommand` | `CreateBlogPostCommand` |
| `GetCategoriesQuery` | `GetCategoriesQuery` |
| `GetTagsQuery` | `GetTagsQuery` |
| `SearchPostsQuery` | `SearchPostsQuery` |
| `GetCommentsQuery` | `GetCommentsQuery` |
| `CreateCommentCommand` | `CreateCommentCommand` |

## Benefits

1. **Exact Mapping**: Operation names match your C# class names exactly
2. **No Transformation**: No conversion or modification of class names
3. **Consistent**: What you see in code is what you use in API calls
4. **Automatic Discovery**: New commands/queries are automatically available
5. **No Configuration**: No need to manually map operation names

## Adding New Operations

To add a new operation, simply create a new command or query class:

```csharp
public class DeleteBlogPostCommand : ICommand<bool>
{
    public int PostId { get; set; }
}
```

The dispatcher will automatically:
- Use the exact class name: `DeleteBlogPostCommand`
- Register it in the operation map
- Handle validation and execution

## Client Usage Example

```javascript
async function callApi(operation, data) {
  const response = await fetch('/api/dispatch', {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify({
      operation: operation,
      data: JSON.stringify(data)
    })
  });
  
  const result = await response.json();
  
  if (!result.isSuccess) {
    const errorMessage = result.errors && result.errors.length > 0 
      ? result.errors.join(', ') 
      : result.message || 'Unknown error occurred';
    throw new Error(errorMessage);
  }
  
  return result.data;
}

// Usage with exact class names
const posts = await callApi('GetBlogPosts', { page: 1, pageSize: 10 });
const user = await callApi('Login', { email: 'user@example.com', password: 'password' });
const comments = await callApi('GetComments', { postId: 1 });
```

## Response Format

All responses follow the `BaseResponse<T>` format:

### Success Response
```json
{
  "isSuccess": true,
  "message": "Operation completed successfully",
  "data": { /* operation-specific response data */ },
  "errors": [],
  "statusCode": 200
}
```

### Error Response
```json
{
  "isSuccess": false,
  "message": "Operation failed",
  "data": null,
  "errors": ["Error message 1", "Error message 2"],
  "statusCode": 400
}
```

### Not Found Response
```json
{
  "isSuccess": false,
  "message": "Resource not found",
  "data": null,
  "errors": [],
  "statusCode": 404
}
``` 