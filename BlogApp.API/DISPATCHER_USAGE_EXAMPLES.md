# Single Dispatcher Endpoint Usage Examples

The API uses a Single Dispatcher Endpoint pattern where all requests go through one endpoint: `POST /api/dispatcher`

## Request Format

All requests use the same format:

```json
{
  "operation": "OperationName",
  "data": {
    // Operation-specific parameters (not JSON string)
  }
}
```

## Available Operations

### Authentication Operations

#### Login
```json
{
  "operation": "LoginCommand",
  "data": {
    "email": "user@example.com",
    "password": "password123"
  }
}
```

#### Register
```json
{
  "operation": "RegisterCommand",
  "data": {
    "email": "newuser@example.com",
    "password": "password123",
    "firstName": "John",
    "lastName": "Doe"
  }
}
```

### Blog Operations

#### Get Blog Posts
```json
{
  "operation": "GetBlogPostsQuery",
  "data": {}
}
```

#### Get Blog Post by Slug
```json
{
  "operation": "GetBlogPostBySlugQuery",
  "data": {
    "slug": "my-blog-post"
  }
}
```

#### Create Blog Post
```json
{
  "operation": "CreateBlogPostCommand",
  "data": {
    "title": "My New Post",
    "content": "Post content...",
    "categoryId": 1,
    "tagIds": [1, 2]
  }
}
```

#### Get Categories
```json
{
  "operation": "GetCategoriesQuery",
  "data": {}
}
```

#### Get Tags
```json
{
  "operation": "GetTagsQuery",
  "data": {}
}
```

#### Search Posts
```json
{
  "operation": "SearchPostsQuery",
  "data": {
    "searchTerm": "keyword",
    "categoryId": 1,
    "tagIds": [1, 2],
    "page": 1,
    "pageSize": 10
  }
}
```

### Comment Operations

#### Get Comments
```json
{
  "operation": "GetCommentsQuery",
  "data": {
    "postId": 1
  }
}
```

#### Create Comment
```json
{
  "operation": "CreateCommentCommand",
  "data": {
    "content": "Great post!",
    "blogPostId": 1,
    "parentCommentId": null
  }
}
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

## Benefits of Single Dispatcher Pattern

1. **Single Entry Point**: All API operations go through one endpoint
2. **Consistent Error Handling**: Centralized error handling and response format
3. **Easy to Extend**: Add new operations by updating the dispatcher
4. **CQRS Integration**: All operations use the mediator pattern
5. **Type Safety**: Strongly typed requests and responses
6. **Clean Architecture**: Clear separation between API layer and business logic

## Client Usage Example (JavaScript)

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

// Usage examples
const posts = await callApi('GetBlogPostsQuery', {});
const user = await callApi('LoginCommand', { email: 'user@example.com', password: 'password' });
const comments = await callApi('GetCommentsQuery', { postId: 1 });
``` 