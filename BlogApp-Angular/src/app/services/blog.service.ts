import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { BlogPost, CreateBlogPost, UpdateBlogPost, Category, Tag, Comment } from '../models/blog-post.model';
import { DispatcherService } from './dispatcher.service';

@Injectable({
  providedIn: 'root'
})
export class BlogService {
  constructor(private dispatcher: DispatcherService) {}

  getPosts(params: any = {}): Observable<BlogPost[]> {
    return this.dispatcher.dynamic['GetBlogPostsQuery'](params);
  }

  getPostBySlug(slug: string): Observable<BlogPost> {
    return this.dispatcher.dynamic['GetBlogPostBySlugQuery']({ slug });
  }

  createPost(post: CreateBlogPost): Observable<BlogPost> {
    return this.dispatcher.dynamic['CreateBlogPostCommand'](post);
  }

  updatePost(post: UpdateBlogPost): Observable<BlogPost> {
    return this.dispatcher.dynamic['UpdateBlogPostCommand'](post);
  }

  deletePost(id: number): Observable<void> {
    return this.dispatcher.dynamic['DeleteBlogPostCommand']({ id });
  }

  getCategories(): Observable<Category[]> {
    return this.dispatcher.dynamic['GetCategoriesQuery']({});
  }

  getTags(): Observable<Tag[]> {
    return this.dispatcher.dynamic['GetTagsQuery']({});
  }

  searchPosts(params: any): Observable<BlogPost[]> {
    return this.dispatcher.dynamic['SearchPostsQuery'](params);
  }

  getComments(postId: number): Observable<Comment[]> {
    return this.dispatcher.dynamic['GetCommentsQuery']({ postId });
  }

  createComment(postId: number, content: string): Observable<Comment> {
    return this.dispatcher.dynamic['CreateCommentCommand']({ blogPostId: postId, content });
  }
} 