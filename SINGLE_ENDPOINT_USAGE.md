# Single Dynamic API Endpoint Usage Guide

আপনার BlogSite API এখন **শুধুমাত্র একটি endpoint** দিয়ে সব কাজ করবে! 🎉

## Main Endpoint
- **URL**: `POST /api`
- **Discovery**: `GET /api` (সব available operations দেখার জন্য)

## Request Format

```json
{
  "action": "getbyauthor|createauthor|updateauthor|deleteauthor|getallauthors|etc",
  "payload": { /* your data */ }
}
```

## ⚡ Dynamic Action Processing

এখন **শুধুমাত্র `action` field** যথেষ্ট! API automatically detect করবে:

- **Operation Type**: Action যদি `get` দিয়ে শুরু হয় = Query, নাহলে = Command  
- **Entity Type**: Action name থেকে automatically parse করা হবে
- **Specific Action**: বাকি part থেকে কি করতে হবে বুঝবে

### Action Naming Convention

#### Query Actions (get দিয়ে শুরু)
- `getallauthors` → GetAllAuthorsQuery
- `getauthorbyid` → GetAuthorByIdQuery  
- `getauthorbyemail` → GetAuthorByEmailQuery
- `getpublishedblogposts` → GetPublishedBlogPostsQuery
- `getblogpostsbycategory` → GetBlogPostsByCategoryQuery
- `getbyauthor` → GetBlogPostsByAuthorQuery
- `getallcategories` → GetAllCategoriesQuery

#### Command Actions (get ছাড়া অন্য)
- `createauthor` → CreateAuthorCommand
- `updateauthor` → UpdateAuthorCommand
- `deleteauthor` → DeleteAuthorCommand
- `createblogpost` → CreateBlogPostCommand
- `publishblogpost` → PublishBlogPostCommand
- `createcategory` → CreateCategoryCommand

## Examples

### 1. Create Author (Command)
```json
{
  "action": "createauthor",
  "payload": {
    "firstName": "John",
    "lastName": "Doe",
    "email": "john@example.com",
    "bio": "Software Developer"
  }
}
```

### 2. Get All Authors (Query)
```json
{
  "action": "getallauthors",
  "payload": {}
}
```

### 3. Get Author By ID
```json
{
  "action": "getauthorbyid",
  "payload": {
    "id": "123e4567-e89b-12d3-a456-426614174000"
  }
}
```

### 4. Create Blog Post
```json
{
  "action": "createblogpost",
  "payload": {
    "title": "My First Post",
    "content": "This is the content of my blog post",
    "summary": "Post summary",
    "authorId": "123e4567-e89b-12d3-a456-426614174000",
    "categoryId": "123e4567-e89b-12d3-a456-426614174001"
  }
}
```

### 5. Update Author
```json
{
  "action": "updateauthor",
  "payload": {
    "id": "123e4567-e89b-12d3-a456-426614174000",
    "firstName": "Updated John",
    "lastName": "Updated Doe",
    "bio": "Senior Software Developer"
  }
}
```

### 6. Publish Blog Post
```json
{
  "action": "publishblogpost",
  "payload": {
    "id": "123e4567-e89b-12d3-a456-426614174002"
  }
}
```

### 7. Get Published Posts
```json
{
  "action": "getpublishedblogposts",
  "payload": {}
}
```

### 8. Get Posts by Author (🆕 Example from your request)
```json
{
  "action": "getbyauthor",
  "payload": {
    "authorId": "123e4567-e89b-12d3-a456-426614174000"
  }
}
```

### 9. Delete Author
```json
{
  "action": "deleteauthor",
  "payload": {
    "id": "123e4567-e89b-12d3-a456-426614174000"
  }
}
```

### 10. Get Author by Email
```json
{
  "action": "getauthorbyemail",
  "payload": {
    "email": "john@example.com"
  }
}
```

### 11. Get BlogPosts by Category
```json
{
  "action": "getblogpostsbycategory",
  "payload": {
    "categoryId": "123e4567-e89b-12d3-a456-426614174001"
  }
}
```

### 12. Create Category
```json
{
  "action": "createcategory",
  "payload": {
    "name": "Technology",
    "description": "Posts about software development"
  }
}
```

## Available Actions

### Author Actions
- `getallauthors` - সব authors পেতে
- `getauthorbyid` - ID দিয়ে author পেতে  
- `getauthorbyemail` - Email দিয়ে author পেতে
- `createauthor` - নতুন author তৈরি করতে
- `updateauthor` - Author update করতে
- `deleteauthor` - Author delete করতে

### BlogPost Actions  
- `getpublishedblogposts` - Published posts পেতে
- `getblogpostsbycategory` - Category অনুযায়ী posts পেতে
- `getbyauthor` - Author অনুযায়ী posts পেতে
- `createblogpost` - নতুন blog post তৈরি করতে
- `publishblogpost` - Blog post publish করতে

### Category Actions
- `getallcategories` - সব categories পেতে
- `createcategory` - নতুন category তৈরি করতে

## Response Format
```json
{
  "id": "123e4567-e89b-12d3-a456-426614174000",
  "firstName": "John",
  "lastName": "Doe",
  "email": "john@example.com",
  "bio": "Software Developer",
  "fullName": "John Doe",
  "createdAt": "2024-01-01T00:00:00Z",
  "updatedAt": "2024-01-01T00:00:00Z",
  "postCount": 0
}
```

## Error Response
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "detail": "Handler type 'GetByAuthorQuery' not found. Available actions: getbyauthor, getauthorbyid, getauthorbyemail, getallauthors, getblogpostsbycategory, getpublishedblogposts, getallcategories, createauthor, updateauthor, deleteauthor, createblogpost, publishblogpost, createcategory"
}
```

## Test করার জন্য
```bash
# Create an author
curl -X POST http://localhost:5000/api \
  -H "Content-Type: application/json" \
  -d '{
    "action": "createauthor",
    "payload": {
      "firstName": "John",
      "lastName": "Doe",
      "email": "john@example.com",
      "bio": "Software Developer"
    }
  }'

# Get all authors
curl -X POST http://localhost:5000/api \
  -H "Content-Type: application/json" \
  -d '{
    "action": "getallauthors",
    "payload": {}
  }'

# Get posts by author (your example)
curl -X POST http://localhost:5000/api \
  -H "Content-Type: application/json" \
  -d '{
    "action": "getbyauthor",
    "payload": {
      "authorId": "123e4567-e89b-12d3-a456-426614174000"
    }
  }'
```

## Benefits of Simplified Single Endpoint
✅ **Super Simple**: শুধু `action` field যথেষ্ট!  
✅ **No More Complex Structure**: `operationType` আর `entityType` এর ঝামেলা নেই  
✅ **Intuitive**: Action name দেখেই বুঝা যায় কি হবে  
✅ **Dynamic**: API automatically সব detect করে  
✅ **Flexible**: নতুন actions সহজেই add করা যায়  
✅ **Developer Friendly**: কম টাইপিং, কম error