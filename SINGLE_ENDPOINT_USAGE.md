# Single Dynamic API Endpoint Usage Guide

আপনার BlogSite API এখন **শুধুমাত্র একটি endpoint** দিয়ে সব কাজ করবে! 🎉

## Main Endpoint
- **URL**: `POST /api`
- **Discovery**: `GET /api` (সব available operations দেখার জন্য)

## Request Format

```json
{
  "operationType": "command|query",
  "entityType": "author|blogpost|category|comment",
  "action": "create|update|delete|get|getall|getbyid|publish|etc",
  "payload": { /* your data */ },
  "parameters": { /* additional parameters */ }
}
```

## Examples

### 1. Create Author (POST)
```json
{
  "operationType": "command",
  "entityType": "author",
  "action": "create",
  "payload": {
    "firstName": "John",
    "lastName": "Doe",
    "email": "john@example.com",
    "bio": "Software Developer"
  }
}
```

### 2. Get All Authors (GET)
```json
{
  "operationType": "query",
  "entityType": "author",
  "action": "getall"
}
```

### 3. Get Author By ID
```json
{
  "operationType": "query",
  "entityType": "author",
  "action": "getbyid",
  "parameters": {
    "id": "123e4567-e89b-12d3-a456-426614174000"
  }
}
```

### 4. Create Blog Post
```json
{
  "operationType": "command",
  "entityType": "blogpost",
  "action": "create",
  "payload": {
    "title": "My First Post",
    "content": "This is the content of my blog post",
    "authorId": "123e4567-e89b-12d3-a456-426614174000",
    "categoryId": "123e4567-e89b-12d3-a456-426614174001"
  }
}
```

### 5. Update Blog Post
```json
{
  "operationType": "command",
  "entityType": "blogpost", 
  "action": "update",
  "payload": {
    "title": "Updated Title",
    "content": "Updated content"
  },
  "parameters": {
    "id": "123e4567-e89b-12d3-a456-426614174002"
  }
}
```

### 6. Publish Blog Post
```json
{
  "operationType": "command",
  "entityType": "blogpost",
  "action": "publish",
  "parameters": {
    "id": "123e4567-e89b-12d3-a456-426614174002"
  }
}
```

### 7. Get Published Posts
```json
{
  "operationType": "query",
  "entityType": "blogpost",
  "action": "getpublished"
}
```

### 8. Get Posts by Author
```json
{
  "operationType": "query",
  "entityType": "blogpost",
  "action": "getbyauthor",
  "parameters": {
    "authorId": "123e4567-e89b-12d3-a456-426614174000"
  }
}
```

### 9. Delete Author
```json
{
  "operationType": "command",
  "entityType": "author",
  "action": "delete",
  "parameters": {
    "id": "123e4567-e89b-12d3-a456-426614174000"
  }
}
```

## Available Entities
- **author**: Author management
- **blogpost**: Blog post management  
- **category**: Category management
- **comment**: Comment management

## Available Operations
- **Commands**: create, update, delete, publish, unpublish
- **Queries**: get, getall, getbyid, getbyemail, getpublished, getbyauthor, getbycategory

## Response Format
```json
{
  "success": true,
  "result": { /* your data */ },
  "operationType": "command",
  "entityType": "author", 
  "action": "create",
  "timestamp": "2024-01-01T00:00:00Z"
}
```

## Error Response
```json
{
  "success": false,
  "error": "Error message",
  "operationType": "command",
  "entityType": "author",
  "action": "create", 
  "timestamp": "2024-01-01T00:00:00Z"
}
```

## Discovery Endpoint
`GET /api` দিয়ে সব available operations দেখতে পারবেন:

```bash
curl -X GET https://your-api-url/api
```

## Test করার জন্য
```bash
# Discover operations
curl -X GET http://localhost:5000/api

# Create an author
curl -X POST http://localhost:5000/api \
  -H "Content-Type: application/json" \
  -d '{
    "operationType": "command",
    "entityType": "author", 
    "action": "create",
    "payload": {
      "firstName": "John",
      "lastName": "Doe",
      "email": "john@example.com",
      "bio": "Software Developer"
    }
  }'
```

## Benefits of Single Endpoint
✅ **Simple**: শুধু একটা endpoint মনে রাখলেই হবে  
✅ **Flexible**: যেকোনো operation dynamically করা যায়  
✅ **Consistent**: সব request একই format এ  
✅ **Discoverable**: GET /api দিয়ে সব operations দেখা যায়  
✅ **Maintainable**: নতুন features যোগ করা সহজ