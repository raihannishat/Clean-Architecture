using FluentValidation;
using AutoRegister;
using BlogApp.API.Application.Features.Comment.DTOs;

namespace BlogApp.API.Application.Features.Comment.Commands;

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