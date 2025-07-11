# BlogApp API Testing Guide (Dynamic Dispatcher)

This guide provides examples of how to test the BlogApp API using the dynamic dispatcher endpoint.

## Base URL
- Development: `https://localhost:7001` or `http://localhost:7000`
- Swagger UI: `https://localhost:7001/swagger`
- Dispatcher Endpoint: `POST /api/dispatcher`

## Authentication

### 1. Register a new user
```bash
curl -X POST "https://localhost:7001/api/dispatcher" \
  -H "Content-Type: application/json" \
  -d '{
    "operation": "RegisterCommand",
    "data": {
      "firstName": "John",
      "lastName": "Doe",
      "email": "john.doe@example.com",
      "password": "Password123!",
      "confirmPassword": "Password123!"
    }
  }'
```

### 2. Login
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

**Response:**
```json
{
  "isSuccess": true,
  "message": "Login successful",
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expiresAt": "2024-01-15T10:30:00Z",
  "user": {
    "id": "user-id",
    "firstName": "Admin",
    "lastName": "User",
    "email": "admin@blogapp.com",
    "bio": "Administrator of BlogApp",
    "profileImageUrl": "",
    "createdAt": "2024-01-08T10:30:00Z"
  }
}
```

## Blog Posts

### 3. Get all published posts
```bash
curl -X POST "https://localhost:7001/api/dispatcher" \
  -H "Content-Type: application/json" \
  -d '{
    "operation": "GetBlogPostsQuery",
    "data": {
      "page": 1,
      "pageSize": 10
    }
  }'
```

### 4. Get post by slug
```bash
curl -X POST "https://localhost:7001/api/dispatcher" \
  -H "Content-Type: application/json" \
  -d '{
    "operation": "GetBlogPostBySlugQuery",
    "data": {
      "slug": "getting-started-with-aspnet-core-8"
    }
  }'
```

### 5. Search posts
```bash
curl -X POST "https://localhost:7001/api/dispatcher" \
  -H "Content-Type: application/json" \
  -d '{
    "operation": "SearchPostsQuery",
    "data": {
      "searchTerm": "aspnet",
      "page": 1,
      "pageSize": 10
    }
  }'
```

### 6. Get categories
```bash
curl -X POST "https://localhost:7001/api/dispatcher" \
  -H "Content-Type: application/json" \
  -d '{
    "operation": "GetCategoriesQuery",
    "data": {}
  }'
```

### 7. Get tags
```bash
curl -X POST "https://localhost:7001/api/dispatcher" \
  -H "Content-Type: application/json" \
  -d '{
    "operation": "GetTagsQuery",
    "data": {}
  }'
```

## Protected Operations (Require Authentication)

### 8. Create a new blog post
```bash
curl -X POST "https://localhost:7001/api/dispatcher" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  -d '{
    "operation": "CreateBlogPostCommand",
    "data": {
      "title": "My New Blog Post",
      "content": "<h2>Introduction</h2><p>This is my new blog post content...</p>",
      "summary": "A brief summary of my new blog post",
      "featuredImageUrl": "https://via.placeholder.com/800x400",
      "categoryId": 1,
      "tagIds": [1, 2],
      "isPublished": true
    }
  }'
```

## Comments

### 9. Get comments for a post
```bash
curl -X POST "https://localhost:7001/api/dispatcher" \
  -H "Content-Type: application/json" \
  -d '{
    "operation": "GetCommentsQuery",
    "data": {
      "postId": 1
    }
  }'
```

### 10. Create a comment
```bash
curl -X POST "https://localhost:7001/api/dispatcher" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  -d '{
    "operation": "CreateCommentCommand",
    "data": {
      "content": "Great article! Thanks for sharing.",
      "blogPostId": 1
    }
  }'
```

### 11. Create a reply to a comment
```bash
curl -X POST "https://localhost:7001/api/dispatcher" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  -d '{
    "operation": "CreateCommentCommand",
    "data": {
      "content": "I agree with your comment!",
      "blogPostId": 1,
      "parentCommentId": 1
    }
  }'
```

## Testing with Postman

1. **Import the collection** (if available)
2. **Set up environment variables:**
   - `base_url`: `https://localhost:7001`
   - `token`: Your JWT token after login

3. **Test flow:**
   1. Register/Login to get a token
   2. Set the token in environment variables
   3. Test protected endpoints

## Testing with JavaScript/Fetch

```javascript
// Generic dispatcher function
async function callDispatcher(operation, data, token = null) {
  const headers = {
    'Content-Type': 'application/json',
  };
  
  if (token) {
    headers['Authorization'] = `Bearer ${token}`;
  }
  
  const response = await fetch('https://localhost:7001/api/dispatcher', {
    method: 'POST',
    headers,
    body: JSON.stringify({
      operation,
      data
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

// Login
async function login(email, password) {
  const result = await callDispatcher('LoginCommand', { email, password });
  return result.token;
}

// Get posts
async function getPosts(token) {
  return await callDispatcher('GetBlogPostsQuery', {}, token);
}

// Create a post
async function createPost(token, postData) {
  return await callDispatcher('CreateBlogPostCommand', postData, token);
}

// Get comments
async function getComments(postId) {
  return await callDispatcher('GetCommentsQuery', { postId });
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
  "message": "Validation failed",
  "data": null,
  "errors": ["Title is required", "Content is required"],
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

## Default Data

The API comes with pre-seeded data:

### Categories
- Technology
- Programming
- Web Development
- Design
- Business
- Lifestyle

### Tags
- C#, ASP.NET Core, JavaScript, React, Angular, Vue.js, Node.js, Python, Docker, Azure, AWS, Database, API, Security, Performance

### Sample Posts
- "Getting Started with ASP.NET Core 8.0"
- "Building a Blog with ASP.NET Core and Entity Framework"

### Default Users
- **Admin**: `admin@blogapp.com` / `Admin123!`
- **Demo**: `demo@blogapp.com` / `Demo123!`

## Notes

- All timestamps are in UTC
- JWT tokens expire after 7 days
- Protected endpoints require the `Authorization: Bearer {token}` header
- Pagination is supported for list endpoints with `page` and `pageSize` parameters
- Search is case-insensitive and searches in title, content, and summary
- Comments are automatically approved for authenticated users 