using BlogSite.Application.DTOs;

namespace BlogSite.Application.Interfaces;

public interface IBlogPostService
{
    Task<IEnumerable<BlogPostDto>> GetAllPostsAsync();
    Task<IEnumerable<BlogPostDto>> GetPublishedPostsAsync();
    Task<BlogPostDto?> GetPostByIdAsync(Guid id);
    Task<IEnumerable<BlogPostDto>> GetPostsByAuthorAsync(Guid authorId);
    Task<IEnumerable<BlogPostDto>> GetPostsByCategoryAsync(Guid categoryId);
    Task<BlogPostDto> CreatePostAsync(CreateBlogPostDto createDto);
    Task<BlogPostDto> UpdatePostAsync(Guid id, UpdateBlogPostDto updateDto);
    Task DeletePostAsync(Guid id);
    Task<BlogPostDto> PublishPostAsync(Guid id);
    Task<BlogPostDto> UnpublishPostAsync(Guid id);
}