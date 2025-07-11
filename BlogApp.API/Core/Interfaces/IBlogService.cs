using BlogApp.API.Application.Common;
using BlogApp.API.Core.Entities;

namespace BlogApp.API.Core.Interfaces;

public interface IBlogService
{
    Task<BaseResponse<List<BlogPost>>> GetBlogPostsAsync(int page = 1, int pageSize = 10, string? category = null, string? tag = null, string? searchTerm = null, bool includeUnpublished = false);
    Task<BaseResponse<BlogPost?>> GetBlogPostBySlugAsync(string slug, bool includeUnpublished = false);
    Task<BaseResponse<List<Category>>> GetCategoriesAsync();
    Task<BaseResponse<List<Tag>>> GetTagsAsync();
    Task<BaseResponse<List<BlogPost>>> SearchPostsAsync(string searchTerm, int page = 1, int pageSize = 10, bool includeUnpublished = false);
} 