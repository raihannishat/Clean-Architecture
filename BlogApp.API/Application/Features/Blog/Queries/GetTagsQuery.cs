namespace BlogApp.API.Application.Features.Blog.Queries;

public record GetTagsQuery(bool IncludeInactive = false) : BlogApp.API.Application.CQRS.IQuery<BaseResponse<List<TagDTO>>>;

[Register(ServiceLifetime.Scoped)]
public class GetTagsQueryHandler : BlogApp.API.Application.CQRS.IQueryHandler<GetTagsQuery, BaseResponse<List<TagDTO>>>
{
    private readonly IQueryUnitOfWork _unitOfWork;
    private readonly IAutoMapper _mapper;

    public GetTagsQueryHandler(IUnitOfWorkFactory unitOfWorkFactory, IAutoMapper mapper)
    {
        _unitOfWork = unitOfWorkFactory.CreateQueryUnitOfWork();
        _mapper = mapper;
    }

    public async Task<BaseResponse<List<TagDTO>>> HandleAsync(GetTagsQuery query, CancellationToken cancellationToken = default)
    {
        var tags = await _unitOfWork.Repository<Core.Entities.Tag>().GetAllAsync();
        var filteredTags = tags.Where(t => query.IncludeInactive || t.IsActive).ToList();
        var tagDtos = filteredTags.Select(t => _mapper.Map<TagDTO>(t)).ToList();
        return BaseResponse<List<TagDTO>>.Success(tagDtos, "Tags retrieved successfully");
    }
} 