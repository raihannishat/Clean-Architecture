import { Component } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';

@Component({
  selector: 'app-nav',
  standalone: true,
  imports: [RouterLink, RouterLinkActive],
  template: `
    <nav class="navbar">
      <div class="nav-container">
        <div class="nav-brand">
          <a routerLink="/">Blog App</a>
        </div>
        
        <div class="nav-links">
          <a routerLink="/blog" routerLinkActive="active" [routerLinkActiveOptions]="{exact: true}">
            Blog List
          </a>
          <a routerLink="/blog/create" routerLinkActive="active">
            Create Post
          </a>
          <a routerLink="/auth/login" routerLinkActive="active">
            Login
          </a>
          <a routerLink="/auth/register" routerLinkActive="active">
            Register
          </a>
          <a routerLink="/dispatcher-example" routerLinkActive="active">
            Dispatcher Demo
          </a>
        </div>
      </div>
    </nav>
  `,
  styles: [`
    .navbar {
      background-color: #333;
      padding: 1rem 0;
      box-shadow: 0 2px 4px rgba(0,0,0,0.1);
    }
    
    .nav-container {
      max-width: 1200px;
      margin: 0 auto;
      display: flex;
      justify-content: space-between;
      align-items: center;
      padding: 0 1rem;
    }
    
    .nav-brand a {
      color: white;
      text-decoration: none;
      font-size: 1.5rem;
      font-weight: bold;
    }
    
    .nav-links {
      display: flex;
      gap: 1rem;
    }
    
    .nav-links a {
      color: white;
      text-decoration: none;
      padding: 0.5rem 1rem;
      border-radius: 4px;
      transition: background-color 0.3s;
    }
    
    .nav-links a:hover,
    .nav-links a.active {
      background-color: #555;
    }
  `]
})
export class NavComponent {} 