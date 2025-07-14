using BlogApp.API.Application.CQRS;
using BlogApp.API.Application.Common;
using BlogApp.API.Core.Entities;
using BlogApp.API.Infrastructure.Persistence.UnitOfWork.Interfaces;
using FluentValidation;
using AutoRegister;

namespace BlogApp.API.Application.Features.Blog.Queries;

public class GetCategoriesQuery : IQuery<BaseResponse<List<Category>>>
{
    public bool IncludeInactive { get; set; } = false;
}

[Register(ServiceLifetime.Scoped)]
public class GetCategoriesQueryHandler : IQueryHandler<GetCategoriesQuery, BaseResponse<List<Category>>>
{
    private readonly IQueryUnitOfWork _unitOfWork;

    public GetCategoriesQueryHandler(IUnitOfWorkFactory unitOfWorkFactory)
    {
        _unitOfWork = unitOfWorkFactory.CreateQueryUnitOfWork();
    }

    public async Task<BaseResponse<List<Category>>> HandleAsync(GetCategoriesQuery query, CancellationToken cancellationToken = default)
    {
        var categories = await _unitOfWork.Repository<Category>().GetAllAsync();
        var filteredCategories = categories.Where(c => query.IncludeInactive || c.IsActive).ToList();

        return BaseResponse<List<Category>>.Success(filteredCategories, "Categories retrieved successfully");
    }
}

[Register(ServiceLifetime.Scoped)]
public class GetCategoriesQueryValidator : AbstractValidator<GetCategoriesQuery>
{
    public GetCategoriesQueryValidator()
    {
        // No validation rules needed for this query
    }
} 