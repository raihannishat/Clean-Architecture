# Single Endpoint API Usage Examples

আপনার প্রোজেক্টে এখন একটি মাত্র endpoint আছে: `/api`

এই endpoint `type` parameter ব্যবহার করে সব operation handle করে।

## কীভাবে কাজ করে

- যদি `type` "Get" দিয়ে শুরু হয়, তাহলে এটি "Query" suffix যোগ করে
- অন্যথায় এটি "Command" suffix যোগ করে

## Usage Examples

### ১. User তৈরি করা (Create User)

```bash
POST /api
Content-Type: application/json

{
  "type": "CreateUser",
  "payload": {
    "name": "John Doe",
    "email": "john@example.com"
  }
}
```

এটি `CreateUserCommand` হ্যান্ডলার খুঁজে বের করবে।

### ২. User পাওয়া (Get User)

```bash
POST /api
Content-Type: application/json

{
  "type": "GetUser",
  "payload": {
    "id": 1
  }
}
```

এটি `GetUserQuery` হ্যান্ডলার খুঁজে বের করবে।

### ৩. Author তৈরি করা

```bash
POST /api
Content-Type: application/json

{
  "type": "CreateAuthor",
  "payload": {
    "name": "লেখকের নাম",
    "email": "author@example.com"
  }
}
```

### ৪. সকল Author পাওয়া

```bash
POST /api
Content-Type: application/json

{
  "type": "GetAllAuthors",
  "payload": {}
}
```

### ৫. BlogPost তৈরি করা

```bash
POST /api
Content-Type: application/json

{
  "type": "CreateBlogPost",
  "payload": {
    "title": "নতুন ব্লগ পোস্ট",
    "content": "এটি একটি নমুনা ব্লগ পোস্ট",
    "authorId": 1,
    "categoryId": 1
  }
}
```

### ৬. BlogPost পাওয়া

```bash
POST /api
Content-Type: application/json

{
  "type": "GetBlogPost",
  "payload": {
    "id": 1
  }
}
```

## Response Format

সফল response:
```json
{
  "id": 1,
  "name": "John Doe",
  "email": "john@example.com",
  "createdAt": "2024-01-01T00:00:00Z"
}
```

Error response:
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "Handler type 'InvalidTypeCommand' not found",
  "status": 500
}
```

## Implementation Details

আপনার dispatcher class:

```csharp
public class Dispatcher
{
    private readonly IServiceProvider _provider;

    public Dispatcher(IServiceProvider provider)
    {
        _provider = provider;
    }

    public async Task<object?> DispatchAsync(string rawType, JsonElement payload)
    {
        string suffix = rawType.StartsWith("Get", StringComparison.OrdinalIgnoreCase)
            ? "Query"
            : "Command";

        string fullTypeName = $"{rawType}{suffix}";

        var type = AppDomain.CurrentDomain
            .GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .FirstOrDefault(t => t.Name.Equals(fullTypeName, StringComparison.Ordinal));

        if (type is null)
            throw new Exception($"Handler type '{fullTypeName}' not found");

        var instance = payload.Deserialize(type)!;

        var resultType = type.GetInterfaces()
            .First(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequest<>))
            .GetGenericArguments()[0];

        var handlerType = typeof(IRequestHandler<,>).MakeGenericType(type, resultType);

        dynamic handler = _provider.GetRequiredService(handlerType);
        return await handler.Handle((dynamic)instance, CancellationToken.None);
    }
}
```

## Available Operations

আপনার প্রোজেক্টে যে Command এবং Query গুলো আছে, সেগুলো এই endpoint দিয়ে ব্যবহার করতে পারবেন:

### Commands (Create, Update, Delete)
- `CreateAuthor` → `CreateAuthorCommand`
- `UpdateAuthor` → `UpdateAuthorCommand`
- `DeleteAuthor` → `DeleteAuthorCommand`
- `CreateBlogPost` → `CreateBlogPostCommand`
- `UpdateBlogPost` → `UpdateBlogPostCommand`
- `DeleteBlogPost` → `DeleteBlogPostCommand`
- `CreateCategory` → `CreateCategoryCommand`
- `UpdateCategory` → `UpdateCategoryCommand`
- `DeleteCategory` → `DeleteCategoryCommand`
- `CreateComment` → `CreateCommentCommand`
- `UpdateComment` → `UpdateCommentCommand`
- `DeleteComment` → `DeleteCommentCommand`

### Queries (Get operations)
- `GetAuthor` → `GetAuthorQuery`
- `GetAllAuthors` → `GetAllAuthorsQuery`
- `GetBlogPost` → `GetBlogPostQuery`
- `GetAllBlogPosts` → `GetAllBlogPostsQuery`
- `GetCategory` → `GetCategoryQuery`
- `GetAllCategories` → `GetAllCategoriesQuery`
- `GetComment` → `GetCommentQuery`
- `GetCommentsByBlogPost` → `GetCommentsByBlogPostQuery`

এখন আপনার একটি মাত্র endpoint দিয়ে সব কাজ হয়ে যাবে! 🎉