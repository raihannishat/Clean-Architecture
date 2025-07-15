using BlogApp.API.Application.CQRS;
using BlogApp.API.Application.Common;
using BlogApp.API.Core.Entities;
using BlogApp.API.Infrastructure.Persistence.UnitOfWork.Interfaces;
using AutoRegister;
using BlogApp.API.Application.Features.Blog.DTOs;
using AutoMapper;

namespace BlogApp.API.Application.Features.Blog.Queries;

public record GetCategoriesQuery(
    bool IncludeInactive = false
) : IQuery<BaseResponse<List<CategoryDTO>>>;

[Register(ServiceLifetime.Scoped)]
public class GetCategoriesQueryHandler : IQueryHandler<GetCategoriesQuery, BaseResponse<List<CategoryDTO>>>
{
    private readonly IQueryUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetCategoriesQueryHandler(IUnitOfWorkFactory unitOfWorkFactory, IMapper mapper)
    {
        _unitOfWork = unitOfWorkFactory.CreateQueryUnitOfWork();
        _mapper = mapper;
    }

    public async Task<BaseResponse<List<CategoryDTO>>> HandleAsync(GetCategoriesQuery query, CancellationToken cancellationToken = default)
    {
        var categories = await _unitOfWork.Repository<Category>().GetAllAsync();
        var filteredCategories = categories.Where(c => query.IncludeInactive || c.IsActive).ToList();
        var categoryDtos = filteredCategories.Select(c => _mapper.Map<CategoryDTO>(c)).ToList();
        return BaseResponse<List<CategoryDTO>>.Success(categoryDtos, "Categories retrieved successfully");
    }
} 