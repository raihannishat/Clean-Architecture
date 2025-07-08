using MediatR;
using BlogSite.Application.DTOs;

namespace BlogSite.Application.Queries.BlogPosts;

public record GetPublishedBlogPostsQuery() : IRequest<IEnumerable<BlogPostDto>>;