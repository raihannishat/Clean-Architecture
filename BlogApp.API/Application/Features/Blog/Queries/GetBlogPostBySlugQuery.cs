namespace BlogApp.API.Application.Features.Blog.Queries;

public record GetBlogPostBySlugQuery(string Slug) : BlogApp.API.Application.CQRS.IQuery<BaseResponse<BlogPostDTO>>;

[Register(ServiceLifetime.Scoped)]
public class GetBlogPostBySlugQueryHandler : BlogApp.API.Application.CQRS.IQueryHandler<GetBlogPostBySlugQuery, BaseResponse<BlogPostDTO>>
{
    private readonly IQueryUnitOfWork _unitOfWork;
    private readonly IAutoMapper _mapper;

    public GetBlogPostBySlugQueryHandler(IUnitOfWorkFactory unitOfWorkFactory, IAutoMapper mapper)
    {
        _unitOfWork = unitOfWorkFactory.CreateQueryUnitOfWork();
        _mapper = mapper;
    }

    public async Task<BaseResponse<BlogPostDTO>> HandleAsync(GetBlogPostBySlugQuery query, CancellationToken cancellationToken = default)
    {
        if (query == null)
        {
            return BaseResponse<BlogPostDTO>.ValidationError(["Query cannot be null"]);
        }
        if (string.IsNullOrWhiteSpace(query.Slug) || query.Slug.Length < 3 || query.Slug.Length > 100)
        {
            return BaseResponse<BlogPostDTO>.ValidationError(["Slug is required and must be between 3 and 100 characters"]);
        }
        var posts = await _unitOfWork.Repository<BlogPost>().GetAllAsync();
        var blogPost = posts.FirstOrDefault(p => p.Slug == query.Slug);
        
        if (blogPost == null)
        {
            return BaseResponse<BlogPostDTO>.NotFound($"Blog post with slug '{query.Slug}' not found");
        }

        var blogPostDto = _mapper.Map<BlogPostDTO>(blogPost);
        return BaseResponse<BlogPostDTO>.Success(blogPostDto, "Blog post retrieved successfully");
    }
} 