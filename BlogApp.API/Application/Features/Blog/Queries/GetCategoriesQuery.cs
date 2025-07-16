namespace BlogApp.API.Application.Features.Blog.Queries;

public record GetCategoriesQuery(bool IncludeInactive = false) : BlogApp.API.Application.CQRS.IQuery<BaseResponse<List<CategoryDTO>>>;

[Register(ServiceLifetime.Scoped)]
public class GetCategoriesQueryHandler : BlogApp.API.Application.CQRS.IQueryHandler<GetCategoriesQuery, BaseResponse<List<CategoryDTO>>>
{
    private readonly IQueryUnitOfWork _unitOfWork;
    private readonly IAutoMapper _mapper;

    public GetCategoriesQueryHandler(IUnitOfWorkFactory unitOfWorkFactory, IAutoMapper mapper)
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