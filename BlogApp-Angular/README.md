# BlogApp Angular Frontend

This is the Angular frontend for the BlogApp project, built with Angular 17 and TypeScript. It uses a dynamic dispatcher service to communicate with the .NET Web API backend.

## Prerequisites

Before running this project, make sure you have the following installed:

- **Node.js** (version 18 or higher)
- **npm** (comes with Node.js)
- **Angular CLI** (version 17 or higher)

## Installation

1. **Install Node.js and npm**
   - Download and install from [nodejs.org](https://nodejs.org/)

2. **Install Angular CLI globally**
   ```bash
   npm install -g @angular/cli
   ```

3. **Install project dependencies**
   ```bash
   cd BlogApp-Angular
   npm install
   ```

## Development Server

Run the development server:

```bash
ng serve
```

Navigate to `http://localhost:4200/`. The application will automatically reload if you change any of the source files.

## Build

Run `ng build` to build the project. The build artifacts will be stored in the `dist/` directory.

```bash
ng build
```

For production build:

```bash
ng build --configuration production
```

## Project Structure

```
src/
├── app/
│   ├── features/
│   │   ├── auth/
│   │   │   ├── login/
│   │   │   │   └── login.component.ts
│   │   │   ├── register/
│   │   │   │   └── register.component.ts
│   │   │   └── auth.routes.ts
│   │   └── blog/
│   │       ├── blog-list/
│   │       │   └── blog-list.component.ts
│   │       ├── blog-create/
│   │       │   └── blog-create.component.ts
│   │       ├── blog-edit/
│   │       │   └── blog-edit.component.ts
│   │       ├── blog-detail/
│   │       │   └── blog-detail.component.ts
│   │       └── blog.routes.ts
│   ├── models/
│   │   ├── auth.model.ts
│   │   └── blog-post.model.ts
│   ├── services/
│   │   ├── auth.service.ts
│   │   ├── blog.service.ts
│   │   └── dispatcher.service.ts
│   ├── interceptors/
│   │   └── auth.interceptor.ts
│   ├── components/
│   │   ├── nav.component.ts
│   │   └── dispatcher-example.component.ts
│   ├── app.component.ts
│   ├── app.config.ts
│   └── app.routes.ts
├── assets/
├── index.html
├── main.ts
└── styles.scss
```

## Routes

- `/blog` — Blog List
- `/blog/create` — Create Blog Post
- `/blog/edit/:id` — Edit Blog Post
- `/blog/:slug` — Blog Post Details
- `/auth/login` — Login
- `/auth/register` — Register
- `/dispatcher-example` — Dispatcher Service Demo

## Features

### Authentication
- User login and registration
- JWT token management
- Protected routes
- Reactive user state with BehaviorSubject
- LocalStorage-based session persistence

### Blog Management
- View all blog posts
- Create new blog posts
- Edit blog posts
- View blog post details
- Add comments to blog posts
- Search and filter posts
- Tag and category selection for posts
- Delete blog posts

### Comment System
- View comments for a blog post
- Add new comments

### Tag & Category Management
- Fetch and select categories and tags for posts

### Dynamic Dispatcher Service
- Single service for all API operations
- Dynamic method creation using Proxy
- Automatic error handling and response mapping
- Type-safe operation calls
- Direct and generic dispatch usage (see Dispatcher Example)

### UI Components
- Modern responsive interface
- Navigation bar with route links
- Loading states
- Error handling
- Form validation
- Dispatcher Example component for API usage demonstration

## API Configuration

**Note:**
- The `operationName` sent to the API must match the CQRS class name (including Command/Query suffix) exactly, with no prefix. For example: `GetBlogPostsQuery`, `CreateBlogPostCommand`, `LoginCommand`.

The application is configured to connect to the .NET Web API running on `https://localhost:7001`. Make sure your API server is running before using the frontend.

The application uses a dynamic dispatcher service that communicates with the single `/api/dispatcher` endpoint. All API operations are routed through this endpoint using operation names.

To change the API URL, update the `apiUrl` property in the dispatcher service:
- `src/app/services/dispatcher.service.ts`

## Running Tests

```bash
ng test
```

## Code Scaffolding

Run `ng generate component component-name` to generate a new component. You can also use `ng generate directive|pipe|service|class|guard|interface|enum|module`.

## Further Help

To get more help on the Angular CLI use `ng help` or go check out the [Angular CLI Overview and Command Reference](https://angular.io/cli) page.

## Dependencies

### Core Dependencies
- **@angular/core**: ^17.0.0
- **@angular/common**: ^17.0.0
- **@angular/router**: ^17.0.0
- **@angular/forms**: ^17.0.0
- **rxjs**: ~7.8.0

### Development Dependencies
- **@angular/cli**: ^17.0.0
- **@angular-devkit/build-angular**: ^17.0.0
- **typescript**: ~5.2.0

## Browser Support

This application supports all modern browsers including:
- Chrome (latest)
- Firefox (latest)
- Safari (latest)
- Edge (latest) 