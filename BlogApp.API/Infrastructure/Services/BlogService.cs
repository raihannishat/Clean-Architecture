using BlogApp.API.Application.Common;
using BlogApp.API.Core.Entities;
using BlogApp.API.Core.Interfaces;
using BlogApp.API.Infrastructure.Persistence.UnitOfWork.Interfaces;
using AutoRegister;

namespace BlogApp.API.Infrastructure.Services;

[Register(ServiceLifetime.Scoped)]
public class BlogService : IBlogService
{
    private readonly IQueryUnitOfWork _unitOfWork;

    public BlogService(IQueryUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<BaseResponse<List<BlogPost>>> GetBlogPostsAsync(int page = 1, int pageSize = 10, string? category = null, string? tag = null, string? searchTerm = null, bool includeUnpublished = false)
    {
        var posts = await _unitOfWork.Repository<BlogPost>().GetAllAsync();
        return BaseResponse<List<BlogPost>>.Success(posts.ToList(), "Blog posts retrieved successfully");
    }

    public async Task<BaseResponse<BlogPost?>> GetBlogPostBySlugAsync(string slug, bool includeUnpublished = false)
    {
        var posts = await _unitOfWork.Repository<BlogPost>().GetAllAsync();
        var post = posts.FirstOrDefault(p => p.Slug == slug);
        if (post == null)
        {
            return BaseResponse<BlogPost?>.NotFound($"Blog post with slug '{slug}' not found");
        }
        return BaseResponse<BlogPost?>.Success(post, "Blog post retrieved successfully");
    }

    public async Task<BaseResponse<List<Category>>> GetCategoriesAsync()
    {
        var categories = await _unitOfWork.Repository<Category>().GetAllAsync();
        return BaseResponse<List<Category>>.Success(categories.ToList(), "Categories retrieved successfully");
    }

    public async Task<BaseResponse<List<Tag>>> GetTagsAsync()
    {
        var tags = await _unitOfWork.Repository<Tag>().GetAllAsync();
        return BaseResponse<List<Tag>>.Success(tags.ToList(), "Tags retrieved successfully");
    }

    public async Task<BaseResponse<List<BlogPost>>> SearchPostsAsync(string searchTerm, int page = 1, int pageSize = 10, bool includeUnpublished = false)
    {
        var posts = await _unitOfWork.Repository<BlogPost>().GetAllAsync();
        var filteredPosts = posts.Where(p => 
            p.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) || 
            p.Content.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
        ).ToList();
        return BaseResponse<List<BlogPost>>.Success(filteredPosts, "Search completed successfully");
    }
} 