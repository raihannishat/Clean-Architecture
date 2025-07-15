using BlogApp.API.Application.CQRS;
using BlogApp.API.Application.Common;
using BlogApp.API.Core.Entities;
using BlogApp.API.Infrastructure.Persistence.UnitOfWork.Interfaces;
using AutoRegister;
using BlogApp.API.Application.Features.Comment.DTOs;
using AutoMapper;

namespace BlogApp.API.Application.Features.Comment.Commands;

public record CreateCommentCommand(
    string Content,
    int BlogPostId,
    string AuthorId,
    int? ParentCommentId
) : ICommand<BaseResponse<CommentDTO>>;

[Register(ServiceLifetime.Scoped)]
public class CreateCommentCommandHandler : ICommandHandler<CreateCommentCommand, BaseResponse<CommentDTO>>
{
    private readonly ICommandUnitOfWork _unitOfWork;
    private readonly IOutboxService _outboxService;
    private readonly IMapper _mapper;

    public CreateCommentCommandHandler(IUnitOfWorkFactory unitOfWorkFactory, IOutboxService outboxService, IMapper mapper)
    {
        _unitOfWork = unitOfWorkFactory.CreateCommandUnitOfWork();
        _outboxService = outboxService;
        _mapper = mapper;
    }

    public async Task<BaseResponse<CommentDTO>> HandleAsync(CreateCommentCommand command, CancellationToken cancellationToken = default)
    {
        var blogPost = await _unitOfWork.Repository<BlogPost>().GetByIdAsync(command.BlogPostId);
        if (blogPost == null)
        {
            return BaseResponse<CommentDTO>.NotFound($"Blog post with ID {command.BlogPostId} not found");
        }

        if (command.ParentCommentId.HasValue)
        {
            var parentComment = await _unitOfWork.Repository<Core.Entities.Comment>().GetByIdAsync(command.ParentCommentId.Value);
            if (parentComment == null)
            {
                return BaseResponse<CommentDTO>.NotFound($"Parent comment with ID {command.ParentCommentId.Value} not found");
            }
        }

        var comment = new Core.Entities.Comment
        {
            Content = command.Content,
            BlogPostId = command.BlogPostId,
            AuthorId = command.AuthorId,
            ParentCommentId = command.ParentCommentId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Repository<Core.Entities.Comment>().AddAsync(comment);
        await _unitOfWork.SaveChangesAsync();
        await _outboxService.AddAsync(nameof(CreateCommentCommand), command, cancellationToken);
        var commentDto = _mapper.Map<CommentDTO>(comment);
        return BaseResponse<CommentDTO>.Success(commentDto, "Comment created successfully");
    }
} 