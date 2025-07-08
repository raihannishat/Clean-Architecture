using MediatR;
using BlogSite.Application.DTOs;

namespace BlogSite.Application.Commands.BlogPosts;

public record CreateBlogPostCommand(
    string Title,
    string Content,
    string? Summary,
    Guid AuthorId,
    Guid CategoryId
) : IRequest<BlogPostDto>;