using BlogApp.API.Application.CQRS;
using BlogApp.API.Application.Common;
using BlogApp.API.Core.Entities;
using BlogApp.API.Infrastructure.Persistence.UnitOfWork.Interfaces;
using FluentValidation;
using AutoRegister;

namespace BlogApp.API.Application.Features.Blog.Queries;

public class GetBlogPostBySlugQuery : IQuery<BaseResponse<BlogPost?>>
{
    public string Slug { get; set; } = string.Empty;
    public bool IncludeUnpublished { get; set; } = false;
}

[Register(ServiceLifetime.Scoped)]
public class GetBlogPostBySlugQueryHandler : IQueryHandler<GetBlogPostBySlugQuery, BaseResponse<BlogPost?>>
{
    private readonly IQueryUnitOfWork _unitOfWork;

    public GetBlogPostBySlugQueryHandler(IUnitOfWorkFactory unitOfWorkFactory)
    {
        _unitOfWork = unitOfWorkFactory.CreateQueryUnitOfWork();
    }

    public async Task<BaseResponse<BlogPost?>> HandleAsync(GetBlogPostBySlugQuery query, CancellationToken cancellationToken = default)
    {
        var posts = await _unitOfWork.Repository<BlogPost>().GetAllAsync();
        var blogPost = posts.FirstOrDefault(p => p.Slug == query.Slug);
        
        if (blogPost == null)
        {
            return BaseResponse<BlogPost?>.NotFound($"Blog post with slug '{query.Slug}' not found");
        }

        return BaseResponse<BlogPost?>.Success(blogPost, "Blog post retrieved successfully");
    }
}

[Register(ServiceLifetime.Scoped)]
public class GetBlogPostBySlugQueryValidator : AbstractValidator<GetBlogPostBySlugQuery>
{
    public GetBlogPostBySlugQueryValidator()
    {
        RuleFor(x => x.Slug)
            .NotEmpty().WithMessage("Slug is required")
            .Matches("^[a-z0-9]+(?:-[a-z0-9]+)*$").WithMessage("Slug must be URL-friendly");
    }
} 