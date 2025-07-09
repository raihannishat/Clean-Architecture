using MediatR;
using BlogSite.Application.DTOs;
using BlogSite.Application.Attributes;

namespace BlogSite.Application.Queries.BlogPosts;

[OperationDescription("Retrieves all blog posts that have been published and are visible to readers", "Get Published Posts")]
public record GetPublishedBlogPostsQuery() : IRequest<IEnumerable<BlogPostDto>>;