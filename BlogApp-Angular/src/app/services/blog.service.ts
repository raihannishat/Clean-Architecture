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
    return this.dispatcher.dynamic['GetBlogPosts'](params);
  }

  getPostBySlug(slug: string): Observable<BlogPost> {
    return this.dispatcher.dynamic['GetBlogPostBySlug']({ slug });
  }

  createPost(post: CreateBlogPost): Observable<BlogPost> {
    return this.dispatcher.dynamic['CreateBlogPost'](post);
  }

  updatePost(post: UpdateBlogPost): Observable<BlogPost> {
    return this.dispatcher.dynamic['UpdateBlogPost'](post);
  }

  deletePost(id: number): Observable<void> {
    return this.dispatcher.dynamic['DeleteBlogPost']({ id });
  }

  getCategories(): Observable<Category[]> {
    return this.dispatcher.dynamic['GetCategories']({});
  }

  getTags(): Observable<Tag[]> {
    return this.dispatcher.dynamic['GetTags']({});
  }

  searchPosts(params: any): Observable<BlogPost[]> {
    return this.dispatcher.dynamic['SearchPosts'](params);
  }

  getComments(postId: number): Observable<Comment[]> {
    return this.dispatcher.dynamic['GetComments']({ postId });
  }

  createComment(postId: number, content: string): Observable<Comment> {
    return this.dispatcher.dynamic['CreateComment']({ blogPostId: postId, content });
  }
} 