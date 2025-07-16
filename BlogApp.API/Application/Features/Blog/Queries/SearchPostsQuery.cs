namespace BlogApp.API.Application.Features.Blog.Queries;

public record SearchPostsQuery(
    string SearchTerm,
    int Page = 1,
    int PageSize = 10,
    bool IncludeUnpublished = false
) : BlogApp.API.Application.CQRS.IQuery<BaseResponse<List<BlogPostListDTO>>>;

[Register(ServiceLifetime.Scoped)]
public class SearchPostsQueryHandler : BlogApp.API.Application.CQRS.IQueryHandler<SearchPostsQuery, BaseResponse<List<BlogPostListDTO>>>
{
    private readonly IQueryUnitOfWork _unitOfWork;
    private readonly IAutoMapper _mapper;

    public SearchPostsQueryHandler(IUnitOfWorkFactory unitOfWorkFactory, IAutoMapper mapper)
    {
        _unitOfWork = unitOfWorkFactory.CreateQueryUnitOfWork();
        _mapper = mapper;
    }

    public async Task<BaseResponse<List<BlogPostListDTO>>> HandleAsync(SearchPostsQuery query, CancellationToken cancellationToken = default)
    {
        if (query == null)
        {
            return BaseResponse<List<BlogPostListDTO>>.ValidationError(["Query cannot be null"]);
        }
        if (string.IsNullOrWhiteSpace(query.SearchTerm))
        {
            return BaseResponse<List<BlogPostListDTO>>.ValidationError(["Search term is required"]);
        }
        if (query.SearchTerm.Length > 200)
        {
            return BaseResponse<List<BlogPostListDTO>>.ValidationError(["Search term cannot exceed 200 characters"]);
        }
        if (query.Page <= 0)
        {
            return BaseResponse<List<BlogPostListDTO>>.ValidationError(["Page must be greater than 0"]);
        }
        if (query.PageSize <= 0)
        {
            return BaseResponse<List<BlogPostListDTO>>.ValidationError(["Page size must be greater than 0"]);
        }
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

        var postDtos = pagedPosts.Select(p => _mapper.Map<BlogPostListDTO>(p)).ToList();
        return BaseResponse<List<BlogPostListDTO>>.Success(postDtos, "Search completed successfully");
    }
} 