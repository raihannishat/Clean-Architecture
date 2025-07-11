import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatSelectModule } from '@angular/material/select';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatChipsModule } from '@angular/material/chips';
import { BlogService } from '../../../services/blog.service';
import { Category, Tag } from '../../../models/blog-post.model';

@Component({
  selector: 'app-blog-create',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatSelectModule,
    MatCheckboxModule,
    MatProgressSpinnerModule,
    MatChipsModule
  ],
  template: `
    <div class="create-post-container">
      <mat-card>
        <mat-card-header>
          <mat-card-title>Create New Blog Post</mat-card-title>
        </mat-card-header>
        
        <mat-card-content>
          <form [formGroup]="postForm" (ngSubmit)="onSubmit()">
            <mat-form-field appearance="outline" class="full-width">
              <mat-label>Title</mat-label>
              <input matInput formControlName="title" placeholder="Enter post title">
              <mat-error *ngIf="postForm.get('title')?.hasError('required')">
                Title is required
              </mat-error>
            </mat-form-field>

            <mat-form-field appearance="outline" class="full-width">
              <mat-label>Summary</mat-label>
              <textarea matInput formControlName="summary" placeholder="Enter post summary" rows="3"></textarea>
              <mat-error *ngIf="postForm.get('summary')?.hasError('required')">
                Summary is required
              </mat-error>
            </mat-form-field>

            <mat-form-field appearance="outline" class="full-width">
              <mat-label>Content</mat-label>
              <textarea matInput formControlName="content" placeholder="Enter post content" rows="10"></textarea>
              <mat-error *ngIf="postForm.get('content')?.hasError('required')">
                Content is required
              </mat-error>
            </mat-form-field>

            <mat-form-field appearance="outline" class="full-width">
              <mat-label>Featured Image URL</mat-label>
              <input matInput formControlName="featuredImageUrl" placeholder="Enter image URL">
            </mat-form-field>

            <mat-form-field appearance="outline" class="full-width">
              <mat-label>Category</mat-label>
              <mat-select formControlName="categoryId">
                <mat-option *ngFor="let category of categories" [value]="category.id">
                  {{ category.name }}
                </mat-option>
              </mat-select>
              <mat-error *ngIf="postForm.get('categoryId')?.hasError('required')">
                Category is required
              </mat-error>
            </mat-form-field>

            <mat-form-field appearance="outline" class="full-width">
              <mat-label>Tags</mat-label>
              <mat-select formControlName="tagIds" multiple>
                <mat-option *ngFor="let tag of tags" [value]="tag.id">
                  {{ tag.name }}
                </mat-option>
              </mat-select>
            </mat-form-field>

            <mat-checkbox formControlName="isPublished" class="full-width">
              Publish immediately
            </mat-checkbox>

            <div class="form-actions">
              <button mat-button type="button" routerLink="/blog">
                Cancel
              </button>
              <button 
                mat-raised-button 
                color="primary" 
                type="submit"
                [disabled]="postForm.invalid || loading">
                <mat-spinner *ngIf="loading" diameter="20"></mat-spinner>
                <span *ngIf="!loading">Create Post</span>
              </button>
            </div>
          </form>
        </mat-card-content>
      </mat-card>
    </div>
  `,
  styles: [`
    .create-post-container {
      max-width: 800px;
      margin: 0 auto;
      padding: 20px;
    }

    .full-width {
      width: 100%;
      margin-bottom: 16px;
    }

    .form-actions {
      display: flex;
      gap: 16px;
      justify-content: flex-end;
      margin-top: 24px;
    }

    textarea {
      resize: vertical;
    }
  `]
})
export class BlogCreateComponent implements OnInit {
  postForm: FormGroup;
  categories: Category[] = [];
  tags: Tag[] = [];
  loading = false;

  constructor(
    private fb: FormBuilder,
    private blogService: BlogService,
    private router: Router
  ) {
    this.postForm = this.fb.group({
      title: ['', Validators.required],
      summary: ['', Validators.required],
      content: ['', Validators.required],
      featuredImageUrl: [''],
      categoryId: ['', Validators.required],
      tagIds: [[]],
      isPublished: [false]
    });
  }

  ngOnInit(): void {
    this.loadCategories();
    this.loadTags();
  }

  loadCategories(): void {
    this.blogService.getCategories().subscribe({
      next: (categories) => {
        this.categories = categories;
      },
      error: () => {
        alert('Failed to load categories');
      }
    });
  }

  loadTags(): void {
    this.blogService.getTags().subscribe({
      next: (tags) => {
        this.tags = tags;
      },
      error: () => {
        alert('Failed to load tags');
      }
    });
  }

  onSubmit(): void {
    if (this.postForm.valid) {
      this.loading = true;
      this.blogService.createPost(this.postForm.value).subscribe({
        next: () => {
          this.router.navigate(['/blog']);
        },
        error: () => {
          this.loading = false;
          alert('Failed to create post');
        }
      });
    }
  }
} 