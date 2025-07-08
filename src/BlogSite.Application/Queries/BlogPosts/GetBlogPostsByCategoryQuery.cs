using MediatR;
using BlogSite.Application.DTOs;

namespace BlogSite.Application.Queries.BlogPosts;

public record GetBlogPostsByCategoryQuery(Guid CategoryId) : IRequest<IEnumerable<BlogPostDto>>;