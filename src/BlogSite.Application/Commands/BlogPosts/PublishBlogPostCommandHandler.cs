using MediatR;
using AutoMapper;
using BlogSite.Application.DTOs;
using BlogSite.Application.Interfaces;

namespace BlogSite.Application.Commands.BlogPosts;

public class PublishBlogPostCommandHandler : IRequestHandler<PublishBlogPostCommand, BlogPostDto>
{
    private readonly IBlogPostRepository _blogPostRepository;
    private readonly IMapper _mapper;

    public PublishBlogPostCommandHandler(IBlogPostRepository blogPostRepository, IMapper mapper)
    {
        _blogPostRepository = blogPostRepository;
        _mapper = mapper;
    }

    public async Task<BlogPostDto> Handle(PublishBlogPostCommand request, CancellationToken cancellationToken)
    {
        var blogPost = await _blogPostRepository.GetByIdAsync(request.Id);
        if (blogPost == null)
        {
            throw new KeyNotFoundException($"Blog post with ID {request.Id} not found.");
        }

        blogPost.IsPublished = true;
        blogPost.PublishedAt = DateTime.UtcNow;
        blogPost.UpdatedAt = DateTime.UtcNow;

        var updatedBlogPost = await _blogPostRepository.UpdateAsync(blogPost);
        return _mapper.Map<BlogPostDto>(updatedBlogPost);
    }
}