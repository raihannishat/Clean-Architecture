using BlogApp.API.Application.CQRS;
using BlogApp.API.Application.Common;
using BlogApp.API.Core.Entities;
using BlogApp.API.Infrastructure.Persistence.UnitOfWork.Interfaces;
using BlogApp.API.Application.Features.Comment.DTOs;
using FluentValidation;
using AutoRegister;

namespace BlogApp.API.Application.Features.Comment.Queries;

public class GetCommentsQuery : IQuery<BaseResponse<List<CommentDTO>>>
{
    public int BlogPostId { get; set; }
    public bool IncludeReplies { get; set; } = true;
}

[Register(ServiceLifetime.Scoped)]
public class GetCommentsQueryHandler : IQueryHandler<GetCommentsQuery, BaseResponse<List<CommentDTO>>>
{
    private readonly IQueryUnitOfWork _unitOfWork;

    public GetCommentsQueryHandler(IUnitOfWorkFactory unitOfWorkFactory)
    {
        _unitOfWork = unitOfWorkFactory.CreateQueryUnitOfWork();
    }

    public async Task<BaseResponse<List<CommentDTO>>> HandleAsync(GetCommentsQuery query, CancellationToken cancellationToken = default)
    {
        var blogPost = await _unitOfWork.Repository<BlogPost>().GetByIdAsync(query.BlogPostId);
        if (blogPost == null)
        {
            return BaseResponse<List<CommentDTO>>.NotFound($"Blog post with ID {query.BlogPostId} not found");
        }

        var comments = await _unitOfWork.Repository<Core.Entities.Comment>().GetAllAsync();
        var filteredComments = comments.Where(c => c.BlogPostId == query.BlogPostId).ToList();

        // Convert to DTOs (simplified mapping)
        var commentDtos = filteredComments.Select(c => new CommentDTO
        {
            Id = c.Id,
            Content = c.Content,
            CreatedAt = c.CreatedAt,
            ParentCommentId = c.ParentCommentId
        }).ToList();

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