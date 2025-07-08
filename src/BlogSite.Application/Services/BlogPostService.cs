using AutoMapper;
using BlogSite.Application.DTOs;
using BlogSite.Application.Interfaces;
using BlogSite.Domain.Entities;

namespace BlogSite.Application.Services;

public class BlogPostService : IBlogPostService
{
    private readonly IBlogPostRepository _repository;
    private readonly IMapper _mapper;

    public BlogPostService(IBlogPostRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<BlogPostDto>> GetAllPostsAsync()
    {
        var posts = await _repository.GetAllAsync();
        return _mapper.Map<IEnumerable<BlogPostDto>>(posts);
    }

    public async Task<IEnumerable<BlogPostDto>> GetPublishedPostsAsync()
    {
        var posts = await _repository.GetPublishedPostsAsync();
        return _mapper.Map<IEnumerable<BlogPostDto>>(posts);
    }

    public async Task<BlogPostDto?> GetPostByIdAsync(Guid id)
    {
        var post = await _repository.GetByIdAsync(id);
        return post != null ? _mapper.Map<BlogPostDto>(post) : null;
    }

    public async Task<IEnumerable<BlogPostDto>> GetPostsByAuthorAsync(Guid authorId)
    {
        var posts = await _repository.GetPostsByAuthorAsync(authorId);
        return _mapper.Map<IEnumerable<BlogPostDto>>(posts);
    }

    public async Task<IEnumerable<BlogPostDto>> GetPostsByCategoryAsync(Guid categoryId)
    {
        var posts = await _repository.GetPostsByCategoryAsync(categoryId);
        return _mapper.Map<IEnumerable<BlogPostDto>>(posts);
    }

    public async Task<BlogPostDto> CreatePostAsync(CreateBlogPostDto createDto)
    {
        var post = _mapper.Map<BlogPost>(createDto);
        var createdPost = await _repository.AddAsync(post);
        
        // Reload to get related data
        var postWithRelations = await _repository.GetByIdAsync(createdPost.Id);
        return _mapper.Map<BlogPostDto>(postWithRelations);
    }

    public async Task<BlogPostDto> UpdatePostAsync(Guid id, UpdateBlogPostDto updateDto)
    {
        var existingPost = await _repository.GetByIdAsync(id);
        if (existingPost == null)
        {
            throw new KeyNotFoundException($"BlogPost with ID {id} not found.");
        }

        _mapper.Map(updateDto, existingPost);
        var updatedPost = await _repository.UpdateAsync(existingPost);
        
        // Reload to get related data
        var postWithRelations = await _repository.GetByIdAsync(updatedPost.Id);
        return _mapper.Map<BlogPostDto>(postWithRelations);
    }

    public async Task DeletePostAsync(Guid id)
    {
        var post = await _repository.GetByIdAsync(id);
        if (post == null)
        {
            throw new KeyNotFoundException($"BlogPost with ID {id} not found.");
        }

        await _repository.DeleteAsync(id);
    }

    public async Task<BlogPostDto> PublishPostAsync(Guid id)
    {
        var post = await _repository.GetByIdAsync(id);
        if (post == null)
        {
            throw new KeyNotFoundException($"BlogPost with ID {id} not found.");
        }

        post.IsPublished = true;
        post.PublishedAt = DateTime.UtcNow;
        
        var updatedPost = await _repository.UpdateAsync(post);
        return _mapper.Map<BlogPostDto>(updatedPost);
    }

    public async Task<BlogPostDto> UnpublishPostAsync(Guid id)
    {
        var post = await _repository.GetByIdAsync(id);
        if (post == null)
        {
            throw new KeyNotFoundException($"BlogPost with ID {id} not found.");
        }

        post.IsPublished = false;
        post.PublishedAt = null;
        
        var updatedPost = await _repository.UpdateAsync(post);
        return _mapper.Map<BlogPostDto>(updatedPost);
    }
}