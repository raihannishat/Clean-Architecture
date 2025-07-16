# Dynamic Dispatcher Integration Guide for Angular

This guide explains how to use the fully dynamic DispatcherEndpoint with your Angular application.

## Overview

The Angular app uses a dynamic `DispatcherService` to communicate with the API's dispatcher endpoint (`/api/dispatch`).

**Important:**
- Client-side should send the operation name **without** any `Command` or `Query` suffix (e.g., `CreateBlogPost`, `GetBlogPosts`, `Login`).
- The API will automatically resolve the correct CQRS type by appending `Command` or `Query` as needed.
- Client-side does **not** need to know or care about Command/Query patterns or suffixes.

**Recent Additions:**
- BlogService now supports dynamic methods for tags, categories, comments, and search (e.g., `getTags`, `getCategories`, `getComments`, `createComment`, `searchPosts`).
- Blog Edit, Blog Detail, Tag/Category selection, and Comment System are all integrated using dynamic dispatcher calls.
- A Dispatcher Example Component (`src/app/components/dispatcher-example.component.ts`) demonstrates advanced and direct usage patterns in the UI.

## Architecture

```
Angular Components → Services → Dynamic DispatcherService → API DispatcherEndpoint → CQRS Handlers
```

## Services Structure

### 1. DispatcherService (Fully Dynamic)
- **Location**: `src/app/services/dispatcher.service.ts`
- **Purpose**: Handles all communication with the API dispatcher
- **Features**: 
  - Generic `dispatch<T>()` method for any operation
  - **Dynamic proxy** that automatically creates methods for any operation name
  - No need to add helper methods manually
  - Error handling and response mapping

### 2. Updated Services
- **BlogService**: Uses dynamic dispatcher methods
  - Includes: getPosts, getPostBySlug, createPost, updatePost, deletePost, getCategories, getTags, searchPosts, getComments, createComment
- **AuthService**: Uses dynamic dispatcher methods

## Usage Patterns

**Note:**
- Always use the operation name without any suffix. For example: `this.dispatcher.dynamic.GetBlogPosts()`, `this.dispatcher.dynamic.Login()`.

**See also:** `dispatcher-example.component.ts` for practical UI usage of all dispatcher patterns, including blog, auth, comments, tags, categories, and custom operations.

### 1. Dynamic Proxy Usage (Recommended)

```typescript
// Blog post লোড করা
this.dispatcher.dynamic.GetBlogPosts({ page: 1, pageSize: 10 })
  .subscribe(posts => {
    // posts: BlogPost[]
  });

// নতুন পোস্ট তৈরি করা
this.dispatcher.dynamic.CreateBlogPost({
  title: 'New Post',
  content: 'Post content',
  categoryId: 1,
  tagIds: [2, 3]
}).subscribe(post => {
  // post: BlogPost
});

// লগইন
this.dispatcher.dynamic.Login({
  email: 'user@example.com',
  password: 'password123'
}).subscribe(response => {
  // response: AuthResponse
});

// Even custom operations work!
this.dispatcher.dynamic.CustomOperation({ param: 'value' })
  .subscribe(result => {
    // result: any
  });
```

### 2. Generic Dispatch Usage

```typescript
// Using the generic dispatch method
this.dispatcher.dispatch<any[]>('GetBlogPostsQuery', { page: 1, pageSize: 10 })
  .subscribe(posts => {
    console.log('Posts:', posts);
  });

// With type safety
this.dispatcher.dispatch<BlogPost[]>('GetBlogPostsQuery', { page: 1 })
  .subscribe(posts => {
    console.log('Typed posts:', posts);
  });
```

### 3. Service Layer Usage

```typescript
// Blog operations
this.blogService.getPosts({ page: 1, pageSize: 10 }).subscribe(posts => {
  console.log('Posts:', posts);
});

this.blogService.createPost(newPost).subscribe(post => {
  console.log('Created post:', post);
});

// Auth operations
this.authService.login(credentials).subscribe(response => {
  console.log('Login successful:', response);
});
```

## How the Dynamic Proxy Works

The `DispatcherService` uses JavaScript's `Proxy` to automatically create methods for any operation name:

```typescript
// When you call:
this.dispatcher.dynamic.GetBlogPostsQuery(params)

// The proxy automatically converts it to:
this.dispatcher.dispatch('GetBlogPostsQuery', params)
```

This means:
- **No manual method additions** required
- **Any operation name** works automatically
- **Type safety** maintained through generics
- **Consistent API** across all operations

## Operation Name Convention

- **Client-side:** Use operation name only (no Command/Query suffix). Example: `CreateBlogPost`, `GetBlogPosts`, `Login`.
- **API-side:** DispatcherEndpoint will append `Command` or `Query` to resolve the correct CQRS type.
- **No prefix or suffix is needed on the client.**

## Available Operations (Automatic)

### Blog Operations
- `GetBlogPostsQuery` - Get paginated blog posts
- `GetBlogPostBySlugQuery` - Get post by slug
- `CreateBlogPostCommand` - Create new blog post
- `UpdateBlogPostCommand` - Update existing post
- `DeleteBlogPostCommand` - Delete post
- `GetCategoriesQuery` - Get all categories
- `GetTagsQuery` - Get all tags
- `SearchPostsQuery` - Search posts with filters
- `GetCommentsQuery` - Get comments for a post
- `CreateCommentCommand` - Create new comment
- `EditBlogPostCommand` - Edit blog post (if implemented in API)

### Auth Operations
- `LoginCommand` - User login
- `RegisterCommand` - User registration

### UI Integration Examples
- Blog Edit: `/blog/edit/:id` uses dynamic dispatcher for loading and updating posts
- Blog Detail: `/blog/:slug` uses dispatcher for post and comments
- Tag/Category: Blog create/edit forms fetch tags/categories dynamically
- Comments: Blog detail and comment forms use dispatcher for loading/creating comments
- Dispatcher Example: `/dispatcher-example` route demonstrates all dispatcher usages interactively

### Custom Operations
- Any operation name you create in the API will work automatically!

## Adding New Operations (Zero Code Changes!)

### 1. Create Command/Query in API
```csharp
public class DeleteUserCommand : ICommand<bool>
{
    public int UserId { get; set; }
}
```

### 2. Use Immediately in Angular (No Service Updates!)
```typescript
// Works immediately - no code changes needed!
this.dispatcher.dynamic.DeleteUserCommand({ userId: 123 })
  .subscribe(success => {
    console.log('User deleted:', success);
  });
```

### 3. Optional: Add to Service for Organization
```typescript
// In UserService
deleteUser(userId: number): Observable<boolean> {
  return this.dispatcher.dynamic.DeleteUserCommand({ userId });
}
```

## Response Format

All API responses follow this format:

```typescript
interface DispatcherResponse {
  success: boolean;
  data: any;
  error: string | null;
}
```

The DispatcherService automatically:
- Extracts `data` from successful responses
- Throws errors for failed responses
- Handles HTTP errors

## Error Handling

### 1. Service Level
```typescript
getPosts(): Observable<BlogPost[]> {
  return this.dispatcher.dynamic.GetBlogPostsQuery().pipe(
    catchError(error => {
      console.error('Failed to load posts:', error);
      return throwError(() => new Error('Failed to load posts'));
    })
  );
}
```

### 2. Component Level
```typescript
loadPosts() {
  this.blogService.getPosts().subscribe({
    next: (posts) => {
      this.posts = posts;
    },
    error: (error) => {
      this.showError('Failed to load posts: ' + error.message);
    }
  });
}
```

### 3. Direct Dynamic Usage
```typescript
this.dispatcher.dynamic.GetBlogPostsQuery().subscribe({
  next: (posts) => {
    console.log('Success:', posts);
  },
  error: (error) => {
    console.error('Error:', error.message);
  }
});
```

## Testing

### Unit Testing Services
```typescript
describe('BlogService', () => {
  let service: BlogService;
  let dispatcher: jasmine.SpyObj<DispatcherService>;

  beforeEach(() => {
    const spy = jasmine.createSpyObj('DispatcherService', ['dispatch']);
    TestBed.configureTestingModule({
      providers: [
        BlogService,
        { provide: DispatcherService, useValue: spy }
      ]
    });
    service = TestBed.inject(BlogService);
    dispatcher = TestBed.inject(DispatcherService) as jasmine.SpyObj<DispatcherService>;
  });

  it('should load posts', () => {
    const mockPosts = [{ id: 1, title: 'Test Post' }];
    dispatcher.dispatch.and.returnValue(of(mockPosts));

    service.getPosts().subscribe(posts => {
      expect(posts).toEqual(mockPosts);
    });

    expect(dispatcher.dispatch).toHaveBeenCalledWith('GetBlogPostsQuery', {});
  });
});
```

### Testing Dynamic Methods
```typescript
describe('DispatcherService', () => {
  let service: DispatcherService;
  let http: jasmine.SpyObj<HttpClient>;

  beforeEach(() => {
    const spy = jasmine.createSpyObj('HttpClient', ['post']);
    TestBed.configureTestingModule({
      providers: [
        DispatcherService,
        { provide: HttpClient, useValue: spy }
      ]
    });
    service = TestBed.inject(DispatcherService);
    http = TestBed.inject(HttpClient) as jasmine.SpyObj<HttpClient>;
  });

  it('should handle dynamic method calls', () => {
    const mockResponse = { success: true, data: [{ id: 1 }], error: null };
    http.post.and.returnValue(of(mockResponse));

    service.dynamic.GetBlogPostsQuery({ page: 1 }).subscribe(result => {
      expect(result).toEqual([{ id: 1 }]);
    });

    expect(http.post).toHaveBeenCalledWith(
      'https://localhost:7001/api/dispatch',
      {
        operation: 'GetBlogPostsQuery',
        data: '{"page":1}'
      }
    );
  });
});
```

## Benefits of Dynamic Approach

1. **Zero Maintenance**: No need to add helper methods for new operations
2. **Automatic Discovery**: Any API operation works immediately
3. **Type Safety**: Maintained through generics
4. **Consistent API**: Same pattern for all operations
5. **Future-Proof**: New operations work without code changes
6. **Clean Code**: Less boilerplate in services
7. **Easy Testing**: Mock single dispatch method

## Migration from Static Approach

If you have existing services using static helper methods:

1. Replace helper method calls with dynamic calls
2. Remove hardcoded helper methods from DispatcherService
3. Update tests to mock the dispatch method
4. Enjoy automatic operation discovery!

## Example Component

See `dispatcher-example.component.ts` for comprehensive examples of:
- Dynamic proxy usage
- Generic dispatch usage
- Service layer integration
- Error handling patterns
- Custom operation examples

## Best Practices

1. **Use Dynamic Proxy**: Prefer `dispatcher.dynamic.OperationName()` for most cases
2. **Service Layer**: Create service methods for complex operations or reusability
3. **Type Safety**: Use generics when possible for better type checking
4. **Error Handling**: Implement consistent error handling patterns
5. **Testing**: Mock the dispatch method for unit tests
6. **Documentation**: Document custom operations in your API documentation 