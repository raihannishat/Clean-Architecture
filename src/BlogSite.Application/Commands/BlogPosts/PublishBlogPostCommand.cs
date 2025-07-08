using MediatR;
using BlogSite.Application.DTOs;

namespace BlogSite.Application.Commands.BlogPosts;

public record PublishBlogPostCommand(Guid Id) : IRequest<BlogPostDto>;