using MediatR;
using AutoMapper;
using BlogSite.Application.DTOs;
using BlogSite.Application.Interfaces;
using BlogSite.Domain.Entities;

namespace BlogSite.Application.Commands.BlogPosts;

public class CreateBlogPostCommandHandler : IRequestHandler<CreateBlogPostCommand, BlogPostDto>
{
    private readonly IBlogPostRepository _blogPostRepository;
    private readonly IAuthorRepository _authorRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMapper _mapper;

    public CreateBlogPostCommandHandler(
        IBlogPostRepository blogPostRepository,
        IAuthorRepository authorRepository,
        ICategoryRepository categoryRepository,
        IMapper mapper)
    {
        _blogPostRepository = blogPostRepository;
        _authorRepository = authorRepository;
        _categoryRepository = categoryRepository;
        _mapper = mapper;
    }

    public async Task<BlogPostDto> Handle(CreateBlogPostCommand request, CancellationToken cancellationToken)
    {
        // Validate author exists
        var author = await _authorRepository.GetByIdAsync(request.AuthorId);
        if (author == null)
        {
            throw new KeyNotFoundException($"Author with ID {request.AuthorId} not found.");
        }

        // Validate category exists
        var category = await _categoryRepository.GetByIdAsync(request.CategoryId);
        if (category == null)
        {
            throw new KeyNotFoundException($"Category with ID {request.CategoryId} not found.");
        }

        var blogPost = new BlogPost
        {
            Title = request.Title,
            Content = request.Content,
            Summary = request.Summary ?? string.Empty,
            AuthorId = request.AuthorId,
            CategoryId = request.CategoryId,
            IsPublished = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var createdBlogPost = await _blogPostRepository.AddAsync(blogPost);
        return _mapper.Map<BlogPostDto>(createdBlogPost);
    }
}