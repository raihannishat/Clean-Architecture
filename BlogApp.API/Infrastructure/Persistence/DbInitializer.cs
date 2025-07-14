using BlogApp.API.Core.Entities;
using BlogApp.API.Infrastructure.Persistence.Contexts;
using BlogApp.API.Infrastructure.Persistence.Factories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.API.Infrastructure.Persistence;

public static class DbInitializer
{
    public static async Task Initialize(IServiceProvider serviceProvider)
    {
        var dbContextFactory = serviceProvider.GetRequiredService<IDbContextFactory>();
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        using var commandContext = dbContextFactory.CreateCommandDbContext();
        using var queryContext = dbContextFactory.CreateQueryDbContext();

        await commandContext.Database.MigrateAsync();
        await queryContext.Database.EnsureCreatedAsync();

        if (!await commandContext.Set<Category>().AnyAsync())
        {
            var categories = new List<Category>
            {
                new Category { Name = "Technology", Description = "Technology related posts", Slug = "technology", IconClass = "fas fa-laptop", Color = "#007bff" },
                new Category { Name = "Programming", Description = "Programming tutorials and tips", Slug = "programming", IconClass = "fas fa-code", Color = "#28a745" },
                new Category { Name = "Web Development", Description = "Web development articles", Slug = "web-development", IconClass = "fas fa-globe", Color = "#dc3545" },
                new Category { Name = "Design", Description = "Design and UI/UX articles", Slug = "design", IconClass = "fas fa-palette", Color = "#ffc107" },
                new Category { Name = "Business", Description = "Business and entrepreneurship", Slug = "business", IconClass = "fas fa-briefcase", Color = "#6f42c1" },
                new Category { Name = "Lifestyle", Description = "Lifestyle and personal development", Slug = "lifestyle", IconClass = "fas fa-heart", Color = "#e83e8c" }
            };

            commandContext.Set<Category>().AddRange(categories);
            await commandContext.SaveChangesAsync();
        }

        if (!await commandContext.Set<Tag>().AnyAsync())
        {
            var tags = new List<Tag>
            {
                new Tag { Name = "C#", Slug = "csharp", Description = "C# programming language", Color = "#178600" },
                new Tag { Name = "ASP.NET Core", Slug = "aspnet-core", Description = "ASP.NET Core framework", Color = "#512BD4" },
                new Tag { Name = "JavaScript", Slug = "javascript", Description = "JavaScript programming", Color = "#F7DF1E" },
                new Tag { Name = "React", Slug = "react", Description = "React library", Color = "#61DAFB" },
                new Tag { Name = "Angular", Slug = "angular", Description = "Angular framework", Color = "#DD0031" },
                new Tag { Name = "Vue.js", Slug = "vuejs", Description = "Vue.js framework", Color = "#4FC08D" },
                new Tag { Name = "Node.js", Slug = "nodejs", Description = "Node.js runtime", Color = "#339933" },
                new Tag { Name = "Python", Slug = "python", Description = "Python programming", Color = "#3776AB" },
                new Tag { Name = "Docker", Slug = "docker", Description = "Docker containers", Color = "#2496ED" },
                new Tag { Name = "Azure", Slug = "azure", Description = "Microsoft Azure", Color = "#0089D6" },
                new Tag { Name = "AWS", Slug = "aws", Description = "Amazon Web Services", Color = "#FF9900" },
                new Tag { Name = "Database", Slug = "database", Description = "Database related topics", Color = "#336791" },
                new Tag { Name = "API", Slug = "api", Description = "Application Programming Interface", Color = "#FF6B6B" },
                new Tag { Name = "Security", Slug = "security", Description = "Security and authentication", Color = "#FF4444" },
                new Tag { Name = "Performance", Slug = "performance", Description = "Performance optimization", Color = "#00C851" }
            };

            commandContext.Set<Tag>().AddRange(tags);
            await commandContext.SaveChangesAsync();
        }

        if (!await commandContext.Set<ApplicationUser>().AnyAsync())
        {
            var adminUser = new ApplicationUser
            {
                UserName = "admin@blogapp.com",
                Email = "admin@blogapp.com",
                FirstName = "Admin",
                LastName = "User",
                EmailConfirmed = true,
                Bio = "Administrator of BlogApp",
                CreatedAt = DateTime.UtcNow
            };

            var result = await userManager.CreateAsync(adminUser, "Admin123!");
            if (result.Succeeded)
            {
                if (!await roleManager.RoleExistsAsync("Admin"))
                {
                    await roleManager.CreateAsync(new IdentityRole("Admin"));
                }
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }

            var demoUser = new ApplicationUser
            {
                UserName = "demo@blogapp.com",
                Email = "demo@blogapp.com",
                FirstName = "Demo",
                LastName = "User",
                EmailConfirmed = true,
                Bio = "Demo user for testing",
                CreatedAt = DateTime.UtcNow
            };

            await userManager.CreateAsync(demoUser, "Demo123!");
        }

        if (!await commandContext.Set<BlogPost>().AnyAsync())
        {
            var adminUser = await commandContext.Set<ApplicationUser>().FirstOrDefaultAsync(u => u.Email == "admin@blogapp.com");
            var technologyCategory = await commandContext.Set<Category>().FirstOrDefaultAsync(c => c.Slug == "technology");
            var programmingCategory = await commandContext.Set<Category>().FirstOrDefaultAsync(c => c.Slug == "programming");
            var csharpTag = await commandContext.Set<Tag>().FirstOrDefaultAsync(t => t.Slug == "csharp");
            var aspnetTag = await commandContext.Set<Tag>().FirstOrDefaultAsync(t => t.Slug == "aspnet-core");

            if (adminUser != null && technologyCategory != null && programmingCategory != null)
            {
                var posts = new List<BlogPost>
                {
                    new BlogPost
                    {
                        Title = "Getting Started with ASP.NET Core 8.0",
                        Content = @"
                            <h2>Introduction</h2>
                            <p>ASP.NET Core 8.0 is the latest version of Microsoft's cross-platform web framework. It brings significant improvements in performance, security, and developer experience.</p>
                            
                            <h2>Key Features</h2>
                            <ul>
                                <li>Improved performance with AOT compilation</li>
                                <li>Enhanced security features</li>
                                <li>Better dependency injection</li>
                                <li>Improved debugging experience</li>
                            </ul>
                            
                            <h2>Getting Started</h2>
                            <p>To create a new ASP.NET Core 8.0 project, run the following command:</p>
                            <pre><code>dotnet new web -n MyApp</code></pre>
                            
                            <p>This will create a new web application with the latest features and templates.</p>
                            
                            <h2>Conclusion</h2>
                            <p>ASP.NET Core 8.0 is a powerful framework for building modern web applications. Its performance improvements and new features make it an excellent choice for both new and existing projects.</p>",
                        Summary = "Learn how to get started with ASP.NET Core 8.0 and explore its new features and improvements.",
                        Slug = "getting-started-with-aspnet-core-8",
                        FeaturedImageUrl = "https://via.placeholder.com/800x400/007bff/ffffff?text=ASP.NET+Core+8.0",
                        CategoryId = programmingCategory.Id,
                        AuthorId = adminUser.Id,
                        IsPublished = true,
                        PublishedAt = DateTime.UtcNow.AddDays(-5),
                        ViewCount = 1250
                    },
                    new BlogPost
                    {
                        Title = "Building a Blog with ASP.NET Core and Entity Framework",
                        Content = @"
                            <h2>Overview</h2>
                            <p>In this tutorial, we'll build a complete blog application using ASP.NET Core and Entity Framework Core. We'll cover everything from database design to user authentication.</p>
                            
                            <h2>Database Design</h2>
                            <p>Our blog will have the following entities:</p>
                            <ul>
                                <li>BlogPost - for storing blog posts</li>
                                <li>Category - for organizing posts</li>
                                <li>Tag - for tagging posts</li>
                                <li>Comment - for user comments</li>
                                <li>User - for user management</li>
                            </ul>
                            
                            <h2>Implementation</h2>
                            <p>We'll use Entity Framework Core for data access and ASP.NET Core Identity for user management. The application will support:</p>
                            <ul>
                                <li>User registration and login</li>
                                <li>Creating and editing blog posts</li>
                                <li>Commenting system</li>
                                <li>Search functionality</li>
                                <li>Category and tag filtering</li>
                            </ul>
                            
                            <h2>Conclusion</h2>
                            <p>This tutorial demonstrates how to build a modern web application using ASP.NET Core and Entity Framework Core.</p>",
                        Summary = "A comprehensive guide to building a blog application using ASP.NET Core and Entity Framework Core.",
                        Slug = "building-blog-aspnet-core-entity-framework",
                        FeaturedImageUrl = "https://via.placeholder.com/800x400/28a745/ffffff?text=Blog+Application",
                        CategoryId = technologyCategory.Id,
                        AuthorId = adminUser.Id,
                        IsPublished = true,
                        PublishedAt = DateTime.UtcNow.AddDays(-3),
                        ViewCount = 890
                    }
                };

                commandContext.Set<BlogPost>().AddRange(posts);
                await commandContext.SaveChangesAsync();

                if (csharpTag != null && aspnetTag != null)
                {
                    var firstPost = posts[0];
                    var blogPostTags = new List<BlogPostTag>
                    {
                        new BlogPostTag { BlogPostId = firstPost.Id, TagId = csharpTag.Id },
                        new BlogPostTag { BlogPostId = firstPost.Id, TagId = aspnetTag.Id }
                    };

                    commandContext.Set<BlogPostTag>().AddRange(blogPostTags);
                    await commandContext.SaveChangesAsync();
                }
            }
        }
    }
} 