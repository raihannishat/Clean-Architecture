using BlogApp.API.Application.CQRS;
using BlogApp.API.Application.Common;
using BlogApp.API.Core.Entities;
using BlogApp.API.Infrastructure.Persistence.UnitOfWork.Interfaces;
using FluentValidation;
using AutoRegister;

namespace BlogApp.API.Application.Features.Blog.Queries;

public class SearchPostsQuery : IQuery<BaseResponse<List<BlogPost>>>
{
    public string SearchTerm { get; set; } = string.Empty;
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public bool IncludeUnpublished { get; set; } = false;
}

[Register(ServiceLifetime.Scoped)]
public class SearchPostsQueryHandler : IQueryHandler<SearchPostsQuery, BaseResponse<List<BlogPost>>>
{
    private readonly IQueryUnitOfWork _unitOfWork;

    public SearchPostsQueryHandler(IUnitOfWorkFactory unitOfWorkFactory)
    {
        _unitOfWork = unitOfWorkFactory.CreateQueryUnitOfWork();
    }

    public async Task<BaseResponse<List<BlogPost>>> HandleAsync(SearchPostsQuery query, CancellationToken cancellationToken = default)
    {
        var posts = await _unitOfWork.Repository<BlogPost>().GetAllAsync();
        var filteredPosts = posts.Where(p => 
            (query.IncludeUnpublished || p.IsPublished) &&
            (p.Title.Contains(query.SearchTerm, StringComparison.OrdinalIgnoreCase) || 
             p.Content.Contains(query.SearchTerm, StringComparison.OrdinalIgnoreCase))
        ).ToList();

        var pagedPosts = filteredPosts
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToList();

        return BaseResponse<List<BlogPost>>.Success(pagedPosts, "Search completed successfully");
    }
}

[Register(ServiceLifetime.Scoped)]
public class SearchPostsQueryValidator : AbstractValidator<SearchPostsQuery>
{
    public SearchPostsQueryValidator()
    {
        RuleFor(x => x.SearchTerm)
            .NotEmpty().WithMessage("Search term is required")
            .MaximumLength(200).WithMessage("Search term cannot exceed 200 characters");

        RuleFor(x => x.Page)
            .GreaterThan(0).WithMessage("Page must be greater than 0");

        RuleFor(x => x.PageSize)
            .GreaterThan(0).WithMessage("Page size must be greater than 0")
            .LessThanOrEqualTo(100).WithMessage("Page size cannot exceed 100");
    }
} 