using BlogApp.API.Application.CQRS;
using BlogApp.API.Application.Common;
using BlogApp.API.Core.Entities;
using BlogApp.API.Infrastructure.Persistence.UnitOfWork.Interfaces;
using FluentValidation;
using AutoRegister;

namespace BlogApp.API.Application.Features.Blog.Queries;

public record GetBlogPostsQuery(
    int Page = 1,
    int PageSize = 10,
    string? Category = null,
    string? Tag = null,
    string? SearchTerm = null,
    bool IncludeUnpublished = false
) : IQuery<BaseResponse<List<BlogPost>>>;

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