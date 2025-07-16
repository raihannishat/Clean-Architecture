# BlogApp API Testing Guide (Dynamic Dispatcher)

> **Note:** All API usage, dispatcher pattern, request/response format, and operation mapping documentation are now located in the `Docs/` directory at the project root. See also:
> - `Docs/DISPATCHER_USAGE_EXAMPLES.md`
> - `Docs/GENERIC_RESPONSE_GUIDE.md`
> - `Docs/PREFIX_BASED_DISPATCHER_USAGE.md`
> - `Docs/BASERESPONSE_MIGRATION_SUMMARY.md`

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
    "operation": "Register",
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
    "operation": "Login",
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
  "token": "...",
  "expiresAt": "...",
  "user": { /* ... */ }
}
```

## Blog Posts

### 3. Get all published posts
```bash
curl -X POST "https://localhost:7001/api/dispatcher" \
  -H "Content-Type: application/json" \
  -d '{
    "operation": "GetBlogPosts",
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
    "operation": "GetBlogPostBySlug",
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
    "operation": "SearchPosts",
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
    "operation": "GetCategories",
    "data": {}
  }'
```

### 7. Get tags
```bash
curl -X POST "https://localhost:7001/api/dispatcher" \
  -H "Content-Type: application/json" \
  -d '{
    "operation": "GetTags",
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
    "operation": "CreateBlogPost",
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
    "operation": "GetComments",
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
    "operation": "CreateComment",
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
    "operation": "CreateComment",
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
// See Docs/DISPATCHER_USAGE_EXAMPLES.md for more details
``` 