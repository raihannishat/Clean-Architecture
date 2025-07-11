import { Routes } from '@angular/router';

export const BLOG_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () => import('./blog-list/blog-list.component').then(m => m.BlogListComponent)
  },
  {
    path: 'create',
    loadComponent: () => import('./blog-create/blog-create.component').then(m => m.BlogCreateComponent)
  },
  {
    path: 'edit/:id',
    loadComponent: () => import('./blog-edit/blog-edit.component').then(m => m.BlogEditComponent)
  },
  {
    path: ':slug',
    loadComponent: () => import('./blog-detail/blog-detail.component').then(m => m.BlogDetailComponent)
  }
]; 