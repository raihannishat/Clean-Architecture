using MediatR;
using BlogSite.Application.DTOs;

namespace BlogSite.Application.Queries.BlogPosts;

public record GetBlogPostsByAuthorQuery(Guid AuthorId) : IRequest<IEnumerable<BlogPostDto>>;