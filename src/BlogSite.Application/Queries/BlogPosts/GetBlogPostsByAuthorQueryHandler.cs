using AutoMapper;
using BlogSite.Application.DTOs;
using BlogSite.Application.Interfaces;
using MediatR;

namespace BlogSite.Application.Queries.BlogPosts;

public class GetBlogPostsByAuthorQueryHandler : IRequestHandler<GetBlogPostsByAuthorQuery, IEnumerable<BlogPostDto>>
{
    private readonly IBlogPostRepository _repository;
    private readonly IMapper _mapper;

    public GetBlogPostsByAuthorQueryHandler(IBlogPostRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<BlogPostDto>> Handle(GetBlogPostsByAuthorQuery request, CancellationToken cancellationToken)
    {
        var posts = await _repository.GetPostsByAuthorAsync(request.AuthorId);
        return _mapper.Map<IEnumerable<BlogPostDto>>(posts);
    }
}