using MediatR;
using AutoMapper;
using BlogSite.Application.DTOs;
using BlogSite.Application.Interfaces;

namespace BlogSite.Application.Queries.BlogPosts;

public class GetPublishedBlogPostsQueryHandler : IRequestHandler<GetPublishedBlogPostsQuery, IEnumerable<BlogPostDto>>
{
    private readonly IBlogPostRepository _blogPostRepository;
    private readonly IMapper _mapper;

    public GetPublishedBlogPostsQueryHandler(IBlogPostRepository blogPostRepository, IMapper mapper)
    {
        _blogPostRepository = blogPostRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<BlogPostDto>> Handle(GetPublishedBlogPostsQuery request, CancellationToken cancellationToken)
    {
        var publishedPosts = await _blogPostRepository.GetPublishedPostsAsync();
        return _mapper.Map<IEnumerable<BlogPostDto>>(publishedPosts);
    }
}