using BlogApp.API.Application.CQRS;
using BlogApp.API.Application.Common;
using BlogApp.API.Core.Entities;
using BlogApp.API.Infrastructure.Persistence.UnitOfWork.Interfaces;
using FluentValidation;
using AutoRegister;

namespace BlogApp.API.Application.Features.Comment.Commands;

public class CreateCommentCommand : ICommand<BaseResponse<Core.Entities.Comment>>
{
    public string Content { get; set; } = string.Empty;
    public int BlogPostId { get; set; }
    public string AuthorId { get; set; } = string.Empty;
    public int? ParentCommentId { get; set; }
}

[Register(ServiceLifetime.Scoped)]
public class CreateCommentCommandHandler : ICommandHandler<CreateCommentCommand, BaseResponse<Core.Entities.Comment>>
{
    private readonly ICommandUnitOfWork _unitOfWork;

    public CreateCommentCommandHandler(IUnitOfWorkFactory unitOfWorkFactory)
    {
        _unitOfWork = unitOfWorkFactory.CreateCommandUnitOfWork();
    }

    public async Task<BaseResponse<Core.Entities.Comment>> HandleAsync(CreateCommentCommand command, CancellationToken cancellationToken = default)
    {
        var blogPost = await _unitOfWork.Repository<BlogPost>().GetByIdAsync(command.BlogPostId);
        if (blogPost == null)
        {
            return BaseResponse<Core.Entities.Comment>.NotFound($"Blog post with ID {command.BlogPostId} not found");
        }

        if (command.ParentCommentId.HasValue)
        {
            var parentComment = await _unitOfWork.Repository<Core.Entities.Comment>().GetByIdAsync(command.ParentCommentId.Value);
            if (parentComment == null)
            {
                return BaseResponse<Core.Entities.Comment>.NotFound($"Parent comment with ID {command.ParentCommentId.Value} not found");
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

        return BaseResponse<Core.Entities.Comment>.Success(comment, "Comment created successfully");
    }
}

[Register(ServiceLifetime.Scoped)]
public class CreateCommentCommandValidator : AbstractValidator<CreateCommentCommand>
{
    public CreateCommentCommandValidator()
    {
        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("Comment content is required")
            .MaximumLength(1000).WithMessage("Comment content cannot exceed 1000 characters");

        RuleFor(x => x.BlogPostId)
            .GreaterThan(0).WithMessage("Blog post ID must be greater than 0");

        RuleFor(x => x.AuthorId)
            .NotEmpty().WithMessage("Author ID is required");

        RuleFor(x => x.ParentCommentId)
            .GreaterThan(0).WithMessage("Parent comment ID must be greater than 0")
            .When(x => x.ParentCommentId.HasValue);
    }
} 