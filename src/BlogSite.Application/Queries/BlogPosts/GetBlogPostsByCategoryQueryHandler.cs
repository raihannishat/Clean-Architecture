using MediatR;
using AutoMapper;
using BlogSite.Application.DTOs;
using BlogSite.Application.Interfaces;

namespace BlogSite.Application.Queries.BlogPosts;

public class GetBlogPostsByCategoryQueryHandler : IRequestHandler<GetBlogPostsByCategoryQuery, IEnumerable<BlogPostDto>>
{
    private readonly IBlogPostRepository _blogPostRepository;
    private readonly IMapper _mapper;

    public GetBlogPostsByCategoryQueryHandler(IBlogPostRepository blogPostRepository, IMapper mapper)
    {
        _blogPostRepository = blogPostRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<BlogPostDto>> Handle(GetBlogPostsByCategoryQuery request, CancellationToken cancellationToken)
    {
        var posts = await _blogPostRepository.GetPostsByCategoryAsync(request.CategoryId);
        return _mapper.Map<IEnumerable<BlogPostDto>>(posts);
    }
}