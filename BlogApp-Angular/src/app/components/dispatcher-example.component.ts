import { Component, OnInit } from '@angular/core';
import { DispatcherService } from '../services/dispatcher.service';
import { BlogService } from '../services/blog.service';
import { AuthService } from '../services/auth.service';

@Component({
  selector: 'app-dispatcher-example',
  template: `
    <div class="dispatcher-example">
      <h2>Dynamic Dispatcher Service Examples</h2>
      
      <!-- Blog Posts -->
      <div class="section">
        <h3>Blog Posts (via Service)</h3>
        <button (click)="loadPosts()">Load Posts</button>
        <button (click)="loadPostBySlug()">Load Post by Slug</button>
        <button (click)="createPost()">Create Post</button>
        <button (click)="searchPosts()">Search Posts</button>
        
        <div *ngIf="posts.length > 0">
          <h4>Posts:</h4>
          <ul>
            <li *ngFor="let post of posts">{{ post.title }}</li>
          </ul>
        </div>
      </div>

      <!-- Categories and Tags -->
      <div class="section">
        <h3>Categories & Tags (via Service)</h3>
        <button (click)="loadCategories()">Load Categories</button>
        <button (click)="loadTags()">Load Tags</button>
        
        <div *ngIf="categories.length > 0">
          <h4>Categories:</h4>
          <ul>
            <li *ngFor="let category of categories">{{ category.name }}</li>
          </ul>
        </div>
        
        <div *ngIf="tags.length > 0">
          <h4>Tags:</h4>
          <ul>
            <li *ngFor="let tag of tags">{{ tag.name }}</li>
          </ul>
        </div>
      </div>

      <!-- Comments -->
      <div class="section">
        <h3>Comments (via Service)</h3>
        <button (click)="loadComments()">Load Comments</button>
        <button (click)="createComment()">Create Comment</button>
        
        <div *ngIf="comments.length > 0">
          <h4>Comments:</h4>
          <ul>
            <li *ngFor="let comment of comments">{{ comment.content }}</li>
          </ul>
        </div>
      </div>

      <!-- Authentication -->
      <div class="section">
        <h3>Authentication (via Service)</h3>
        <button (click)="login()">Login</button>
        <button (click)="register()">Register</button>
        <button (click)="logout()">Logout</button>
        
        <div *ngIf="currentUser">
          <h4>Current User:</h4>
          <p>{{ currentUser.firstName }} {{ currentUser.lastName }}</p>
        </div>
      </div>

      <!-- Direct Dynamic Dispatcher Usage -->
      <div class="section">
        <h3>Direct Dynamic Dispatcher Usage</h3>
        <button (click)="directDynamicDispatch()">Direct Dynamic Dispatch</button>
        <button (click)="directDynamicDispatchWithParams()">With Parameters</button>
        <button (click)="directDynamicDispatchCustom()">Custom Operation</button>
        
        <div *ngIf="directResult">
          <h4>Direct Result:</h4>
          <pre>{{ directResult | json }}</pre>
        </div>
      </div>

      <!-- Generic Dispatch Usage -->
      <div class="section">
        <h3>Generic Dispatch Usage</h3>
        <button (click)="genericDispatch()">Generic Dispatch</button>
        <button (click)="genericDispatchWithType()">With Type Safety</button>
        
        <div *ngIf="genericResult">
          <h4>Generic Result:</h4>
          <pre>{{ genericResult | json }}</pre>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .dispatcher-example {
      padding: 20px;
      max-width: 800px;
      margin: 0 auto;
    }
    
    .section {
      margin-bottom: 30px;
      padding: 20px;
      border: 1px solid #ddd;
      border-radius: 5px;
    }
    
    button {
      margin: 5px;
      padding: 8px 16px;
      background-color: #007bff;
      color: white;
      border: none;
      border-radius: 4px;
      cursor: pointer;
    }
    
    button:hover {
      background-color: #0056b3;
    }
    
    ul {
      list-style-type: none;
      padding: 0;
    }
    
    li {
      padding: 5px 0;
      border-bottom: 1px solid #eee;
    }
    
    pre {
      background-color: #f8f9fa;
      padding: 10px;
      border-radius: 4px;
      overflow-x: auto;
    }
  `]
})
export class DispatcherExampleComponent implements OnInit {
  posts: any[] = [];
  categories: any[] = [];
  tags: any[] = [];
  comments: any[] = [];
  currentUser: any = null;
  directResult: any = null;
  genericResult: any = null;

  constructor(
    private dispatcher: DispatcherService,
    private blogService: BlogService,
    private authService: AuthService
  ) {}

  ngOnInit() {
    this.authService.currentUser$.subscribe(user => {
      this.currentUser = user;
    });
  }

  // Blog Service Examples (using dynamic dispatcher)
  loadPosts() {
    this.blogService.getPosts({ page: 1, pageSize: 10 }).subscribe({
      next: (posts) => {
        this.posts = posts;
        console.log('Posts loaded via service:', posts);
      },
      error: (error) => {
        console.error('Error loading posts:', error);
      }
    });
  }

  loadPostBySlug() {
    this.blogService.getPostBySlug('sample-post').subscribe({
      next: (post) => {
        console.log('Post loaded via service:', post);
      },
      error: (error) => {
        console.error('Error loading post:', error);
      }
    });
  }

  createPost() {
    const newPost = {
      title: 'New Post via Dynamic Dispatcher',
      content: 'This post was created using the dynamic dispatcher service',
      summary: 'A test post',
      categoryId: 1,
      tagIds: [1, 2],
      isPublished: true,
      featuredImageUrl: 'https://example.com/image.jpg'
    };

    this.blogService.createPost(newPost).subscribe({
      next: (post) => {
        console.log('Post created via service:', post);
        this.loadPosts();
      },
      error: (error) => {
        console.error('Error creating post:', error);
      }
    });
  }

  searchPosts() {
    this.blogService.searchPosts({ searchTerm: 'test', page: 1, pageSize: 5 }).subscribe({
      next: (posts) => {
        this.posts = posts;
        console.log('Search results via service:', posts);
      },
      error: (error) => {
        console.error('Error searching posts:', error);
      }
    });
  }

  loadCategories() {
    this.blogService.getCategories().subscribe({
      next: (categories) => {
        this.categories = categories;
        console.log('Categories loaded via service:', categories);
      },
      error: (error) => {
        console.error('Error loading categories:', error);
      }
    });
  }

  loadTags() {
    this.blogService.getTags().subscribe({
      next: (tags) => {
        this.tags = tags;
        console.log('Tags loaded via service:', tags);
      },
      error: (error) => {
        console.error('Error loading tags:', error);
      }
    });
  }

  loadComments() {
    this.blogService.getComments(1).subscribe({
      next: (comments) => {
        this.comments = comments;
        console.log('Comments loaded via service:', comments);
      },
      error: (error) => {
        console.error('Error loading comments:', error);
      }
    });
  }

  createComment() {
    this.blogService.createComment(1, 'This is a test comment via dynamic dispatcher').subscribe({
      next: (comment) => {
        console.log('Comment created via service:', comment);
        this.loadComments();
      },
      error: (error) => {
        console.error('Error creating comment:', error);
      }
    });
  }

  login() {
    const credentials = {
      email: 'test@example.com',
      password: 'password123'
    };

    this.authService.login(credentials).subscribe({
      next: (response) => {
        console.log('Login successful via service:', response);
      },
      error: (error) => {
        console.error('Login failed:', error);
      }
    });
  }

  register() {
    const userData = {
      email: 'newuser@example.com',
      password: 'password123',
      firstName: 'John',
      lastName: 'Doe'
    };

    this.authService.register(userData).subscribe({
      next: (response) => {
        console.log('Registration successful via service:', response);
      },
      error: (error) => {
        console.error('Registration failed:', error);
      }
    });
  }

  logout() {
    this.authService.logout();
    console.log('User logged out');
  }

  // Direct Dynamic Dispatcher Usage Examples
  directDynamicDispatch() {
    // Using the dynamic proxy - any method name works!
    this.dispatcher.dynamic.GetBlogPostsQuery({ page: 1, pageSize: 5 }).subscribe({
      next: (result) => {
        this.directResult = result;
        console.log('Direct dynamic dispatch result:', result);
      },
      error: (error) => {
        console.error('Direct dynamic dispatch error:', error);
      }
    });
  }

  directDynamicDispatchWithParams() {
    // Dynamic method with parameters
    this.dispatcher.dynamic.GetBlogPostBySlugQuery({ slug: 'sample-post' }).subscribe({
      next: (result) => {
        this.directResult = result;
        console.log('Direct dynamic dispatch with params:', result);
      },
      error: (error) => {
        console.error('Direct dynamic dispatch with params error:', error);
      }
    });
  }

  directDynamicDispatchCustom() {
    // Any operation name works dynamically!
    this.dispatcher.dynamic.CustomOperationQuery({ customParam: 'value' }).subscribe({
      next: (result) => {
        this.directResult = result;
        console.log('Custom operation result:', result);
      },
      error: (error) => {
        console.error('Custom operation error:', error);
      }
    });
  }

  // Generic Dispatch Usage Examples
  genericDispatch() {
    // Using the generic dispatch method
    this.dispatcher.dispatch<any[]>('GetBlogPostsQuery', { page: 1, pageSize: 3 }).subscribe({
      next: (result) => {
        this.genericResult = result;
        console.log('Generic dispatch result:', result);
      },
      error: (error) => {
        console.error('Generic dispatch error:', error);
      }
    });
  }

  genericDispatchWithType() {
    // Using the generic dispatch method with type safety
    this.dispatcher.dispatch<{ id: number; title: string; content: string }[]>(
      'GetBlogPostsQuery', 
      { page: 1, pageSize: 2 }
    ).subscribe({
      next: (result) => {
        this.genericResult = result;
        console.log('Generic dispatch with type result:', result);
      },
      error: (error) => {
        console.error('Generic dispatch with type error:', error);
      }
    });
  }
} 