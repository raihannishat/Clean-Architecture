using BlogSite.Application.DTOs;
using BlogSite.Application.Interfaces;
using BlogSite.Application.Mappings;
using MediatR;

namespace BlogSite.Application.Queries.BlogPosts;

public class GetBlogPostsByAuthorQueryHandler : IRequestHandler<GetBlogPostsByAuthorQuery, IEnumerable<BlogPostDto>>
{
    private readonly IBlogPostRepository _repository;

    public GetBlogPostsByAuthorQueryHandler(IBlogPostRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<BlogPostDto>> Handle(GetBlogPostsByAuthorQuery request, CancellationToken cancellationToken)
    {
        var posts = await _repository.GetPostsByAuthorAsync(request.AuthorId);
        return posts.Select(MappingExtensions.MapToDto);
    }
}