

namespace BlogApp.API.Application.Features.Comment.Queries;

public record GetCommentsQuery(
    int BlogPostId,
    bool IncludeReplies = true
) : BlogApp.API.Application.CQRS.IQuery<BaseResponse<List<CommentDTO>>>;

[Register(ServiceLifetime.Scoped)]
public class GetCommentsQueryHandler : BlogApp.API.Application.CQRS.IQueryHandler<GetCommentsQuery, BaseResponse<List<CommentDTO>>>
{
    private readonly IQueryUnitOfWork _unitOfWork;
    private readonly IAutoMapper _mapper;

    public GetCommentsQueryHandler(IUnitOfWorkFactory unitOfWorkFactory, IAutoMapper mapper)
    {
        _unitOfWork = unitOfWorkFactory.CreateQueryUnitOfWork();
        _mapper = mapper;
    }

    public async Task<BaseResponse<List<CommentDTO>>> HandleAsync(GetCommentsQuery query, CancellationToken cancellationToken = default)
    {
        if (query == null)
        {
            return BaseResponse<List<CommentDTO>>.ValidationError(["Query cannot be null"]);
        }
        if (query.BlogPostId <= 0)
        {
            return BaseResponse<List<CommentDTO>>.ValidationError(["Blog post ID must be greater than 0"]);
        }
        var blogPost = await _unitOfWork.Repository<BlogPost>().GetByIdAsync(query.BlogPostId);
        if (blogPost == null)
        {
            return BaseResponse<List<CommentDTO>>.NotFound($"Blog post with ID {query.BlogPostId} not found");
        }
        var comments = await _unitOfWork.Repository<Core.Entities.Comment>().GetAllAsync();
        var filteredComments = comments.Where(c => c.BlogPostId == query.BlogPostId).ToList();
        var commentDtos = filteredComments.Select(c => _mapper.Map<CommentDTO>(c)).ToList();
        return BaseResponse<List<CommentDTO>>.Success(commentDtos, "Comments retrieved successfully");
    }
}

[Register(ServiceLifetime.Scoped)]
public class GetCommentsQueryValidator : AbstractValidator<GetCommentsQuery>
{
    public GetCommentsQueryValidator()
    {
        RuleFor(x => x.BlogPostId)
            .GreaterThan(0).WithMessage("Blog post ID must be greater than 0");
    }
} 