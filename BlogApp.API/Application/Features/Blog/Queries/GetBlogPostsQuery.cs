using BlogApp.API.Application.CQRS;
using BlogApp.API.Application.Common;
using BlogApp.API.Core.Entities;
using BlogApp.API.Infrastructure.Persistence.UnitOfWork.Interfaces;
using FluentValidation;
using AutoRegister;

namespace BlogApp.API.Application.Features.Blog.Queries;

public class GetBlogPostsQuery : IQuery<BaseResponse<List<BlogPost>>>
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? Category { get; set; }
    public string? Tag { get; set; }
    public string? SearchTerm { get; set; }
    public bool IncludeUnpublished { get; set; } = false;
}

[Register(ServiceLifetime.Scoped)]
public class GetBlogPostsQueryHandler : IQueryHandler<GetBlogPostsQuery, BaseResponse<List<BlogPost>>>
{
    private readonly IQueryUnitOfWork _unitOfWork;

    public GetBlogPostsQueryHandler(IUnitOfWorkFactory unitOfWorkFactory)
    {
        _unitOfWork = unitOfWorkFactory.CreateQueryUnitOfWork();
    }

    public async Task<BaseResponse<List<BlogPost>>> HandleAsync(GetBlogPostsQuery query, CancellationToken cancellationToken = default)
    {
        var posts = await _unitOfWork.Repository<BlogPost>().GetAllAsync();

        return BaseResponse<List<BlogPost>>.Success(posts.ToList(), "Blog posts retrieved successfully");
    }
}

[Register(ServiceLifetime.Scoped)]
public class GetBlogPostsQueryValidator : AbstractValidator<GetBlogPostsQuery>
{
    public GetBlogPostsQueryValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThan(0).WithMessage("Page must be greater than 0");

        RuleFor(x => x.PageSize)
            .GreaterThan(0).WithMessage("Page size must be greater than 0")
            .LessThanOrEqualTo(100).WithMessage("Page size cannot exceed 100");

        RuleFor(x => x.Category)
            .MaximumLength(100).WithMessage("Category name cannot exceed 100 characters");

        RuleFor(x => x.Tag)
            .MaximumLength(100).WithMessage("Tag name cannot exceed 100 characters");

        RuleFor(x => x.SearchTerm)
            .MaximumLength(200).WithMessage("Search term cannot exceed 200 characters");
    }
} 