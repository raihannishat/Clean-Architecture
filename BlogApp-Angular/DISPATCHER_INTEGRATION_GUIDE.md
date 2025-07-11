# Dynamic Dispatcher Integration Guide for Angular

This guide explains how to use the fully dynamic DispatcherEndpoint with your Angular application.

## Overview

The Angular app now uses a completely dynamic `DispatcherService` that communicates with the API's dynamic dispatcher endpoint (`/api/dispatch`). This provides a unified way to handle all API operations without requiring manual helper method additions.

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
- **AuthService**: Uses dynamic dispatcher methods

## Usage Patterns

### 1. Dynamic Proxy Usage (Recommended)

```typescript
// Any operation name works automatically!
this.dispatcher.dynamic.GetBlogPostsQuery({ page: 1, pageSize: 10 })
  .subscribe(posts => {
    console.log('Posts:', posts);
  });

this.dispatcher.dynamic.CreateBlogPostCommand(newPost)
  .subscribe(post => {
    console.log('Created post:', post);
  });

this.dispatcher.dynamic.LoginCommand(credentials)
  .subscribe(response => {
    console.log('Login successful:', response);
  });

// Even custom operations work!
this.dispatcher.dynamic.CustomOperationQuery({ param: 'value' })
  .subscribe(result => {
    console.log('Custom result:', result);
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

### Auth Operations
- `LoginCommand` - User login
- `RegisterCommand` - User registration

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