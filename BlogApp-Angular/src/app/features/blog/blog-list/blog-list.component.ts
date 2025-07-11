import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatChipsModule } from '@angular/material/chips';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { BlogService } from '../../../services/blog.service';
import { BlogPost } from '../../../models/blog-post.model';

@Component({
  selector: 'app-blog-list',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatChipsModule,
    MatProgressSpinnerModule
  ],
  template: `
    <div class="blog-list">
      <div class="header">
        <h1>My Blog Posts</h1>
        <button mat-raised-button color="primary" routerLink="/blog/create">
          <mat-icon>add</mat-icon>
          Create Post
        </button>
      </div>

      <div *ngIf="loading" class="loading">
        <mat-spinner></mat-spinner>
      </div>

      <div *ngIf="!loading && posts.length === 0" class="empty-state">
        <mat-icon>article</mat-icon>
        <h3>No blog posts yet</h3>
        <p>Create your first blog post to get started!</p>
        <button mat-raised-button color="primary" routerLink="/blog/create">
          Create Your First Post
        </button>
      </div>

      <div *ngIf="!loading && posts.length > 0" class="posts-grid">
        <mat-card *ngFor="let post of posts" class="post-card">
          <img *ngIf="post.featuredImageUrl" mat-card-image [src]="post.featuredImageUrl" [alt]="post.title">
          <mat-card-header>
            <mat-card-title>{{ post.title }}</mat-card-title>
            <mat-card-subtitle>
              {{ post.categoryName }} • {{ post.createdAt | date:'mediumDate' }}
            </mat-card-subtitle>
          </mat-card-header>
          <mat-card-content>
            <p>{{ post.summary }}</p>
            <div class="tags">
              <mat-chip *ngFor="let tag of post.tags" color="primary" variant="outlined">
                {{ tag }}
              </mat-chip>
            </div>
          </mat-card-content>
          <mat-card-actions>
            <button mat-button [routerLink]="['/blog', post.slug]">
              <mat-icon>visibility</mat-icon>
              View
            </button>
            <button mat-button [routerLink]="['/blog/edit', post.id]">
              <mat-icon>edit</mat-icon>
              Edit
            </button>
            <button mat-button color="warn" (click)="deletePost(post.id)">
              <mat-icon>delete</mat-icon>
              Delete
            </button>
          </mat-card-actions>
        </mat-card>
      </div>
    </div>
  `,
  styles: [`
    .blog-list {
      padding: 20px;
    }

    .header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-bottom: 30px;
    }

    .loading {
      display: flex;
      justify-content: center;
      padding: 50px;
    }

    .empty-state {
      text-align: center;
      padding: 50px;
    }

    .empty-state mat-icon {
      font-size: 64px;
      width: 64px;
      height: 64px;
      color: #ccc;
    }

    .posts-grid {
      display: grid;
      grid-template-columns: repeat(auto-fill, minmax(350px, 1fr));
      gap: 20px;
    }

    .post-card {
      height: 100%;
      display: flex;
      flex-direction: column;
    }

    .post-card mat-card-content {
      flex: 1;
    }

    .tags {
      margin-top: 10px;
    }

    .tags mat-chip {
      margin-right: 5px;
      margin-bottom: 5px;
    }
  `]
})
export class BlogListComponent implements OnInit {
  posts: BlogPost[] = [];
  loading = true;

  constructor(private blogService: BlogService) {}

  ngOnInit(): void {
    this.loadPosts();
  }

  loadPosts(): void {
    this.loading = true;
    this.blogService.getPosts().subscribe({
      next: (posts) => {
        this.posts = posts;
        this.loading = false;
      },
      error: () => {
        this.loading = false;
      }
    });
  }

  deletePost(id: number): void {
    if (confirm('Are you sure you want to delete this post?')) {
      this.blogService.deletePost(id).subscribe({
        next: () => {
          this.posts = this.posts.filter(post => post.id !== id);
        },
        error: () => {
          alert('Failed to delete post');
        }
      });
    }
  }
} 