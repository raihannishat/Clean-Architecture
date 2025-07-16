namespace BlogApp.API.Application.Features.Blog.Queries;

public record GetBlogPostsQuery(
    int Page = 1,
    int PageSize = 10,
    string? Category = null,
    string? Tag = null,
    string? SearchTerm = null,
    bool IncludeUnpublished = false
) : BlogApp.API.Application.CQRS.IQuery<BaseResponse<List<BlogPostDTO>>>;

[Register(ServiceLifetime.Scoped)]
public class GetBlogPostsQueryHandler : BlogApp.API.Application.CQRS.IQueryHandler<GetBlogPostsQuery, BaseResponse<List<BlogPostDTO>>>
{
    private readonly IQueryUnitOfWork _unitOfWork;
    private readonly IAutoMapper _mapper;

    public GetBlogPostsQueryHandler(IUnitOfWorkFactory unitOfWorkFactory, IAutoMapper mapper)
    {
        _unitOfWork = unitOfWorkFactory.CreateQueryUnitOfWork();
        _mapper = mapper;
    }

    public async Task<BaseResponse<List<BlogPostDTO>>> HandleAsync(GetBlogPostsQuery query, CancellationToken cancellationToken = default)
    {
        var posts = await _unitOfWork.Repository<BlogPost>().GetAllAsync();
        var postDtos = posts.Select(p => _mapper.Map<BlogPostDTO>(p)).ToList();
        return BaseResponse<List<BlogPostDTO>>.Success(postDtos, "Blog posts retrieved successfully");
    }
} 