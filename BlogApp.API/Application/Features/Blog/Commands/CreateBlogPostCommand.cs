using BlogApp.API.Application.CQRS;
using BlogApp.API.Application.Common;
using BlogApp.API.Core.Entities;
using BlogApp.API.Infrastructure.Persistence.UnitOfWork.Interfaces;
using FluentValidation;
using AutoRegister;

namespace BlogApp.API.Application.Features.Blog.Commands;

public class CreateBlogPostCommand : ICommand<BaseResponse<BlogPost>>
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public int CategoryId { get; set; }
    public List<int> TagIds { get; set; } = new();
    public string AuthorId { get; set; } = string.Empty;
}

[Register(ServiceLifetime.Scoped)]
public class CreateBlogPostCommandHandler : ICommandHandler<CreateBlogPostCommand, BaseResponse<BlogPost>>
{
    private readonly ICommandUnitOfWork _unitOfWork;

    public CreateBlogPostCommandHandler(IUnitOfWorkFactory unitOfWorkFactory)
    {
        _unitOfWork = unitOfWorkFactory.CreateCommandUnitOfWork();
    }

    public async Task<BaseResponse<BlogPost>> HandleAsync(CreateBlogPostCommand command, CancellationToken cancellationToken = default)
    {
        var category = await _unitOfWork.Repository<Category>().GetByIdAsync(command.CategoryId);
        if (category == null)
        {
            return BaseResponse<BlogPost>.NotFound($"Category with ID {command.CategoryId} not found");
        }

        var tags = new List<Tag>();
        foreach (var tagId in command.TagIds)
        {
            var tag = await _unitOfWork.Repository<Tag>().GetByIdAsync(tagId);
            if (tag == null)
            {
                return BaseResponse<BlogPost>.NotFound($"Tag with ID {tagId} not found");
            }
            tags.Add(tag);
        }

        var blogPost = new BlogPost
        {
            Title = command.Title,
            Content = command.Content,
            Slug = command.Slug,
            CategoryId = command.CategoryId,
            AuthorId = command.AuthorId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Repository<BlogPost>().AddAsync(blogPost);

        foreach (var tag in tags)
        {
            var blogPostTag = new BlogPostTag
            {
                BlogPostId = blogPost.Id,
                TagId = tag.Id
            };
            
            await _unitOfWork.Repository<BlogPostTag>().AddAsync(blogPostTag);
        }

        await _unitOfWork.SaveChangesAsync();

        return BaseResponse<BlogPost>.Success(blogPost, "Blog post created successfully");
    }
}

[Register(ServiceLifetime.Scoped)]
public class CreateBlogPostCommandValidator : AbstractValidator<CreateBlogPostCommand>
{
    public CreateBlogPostCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .MaximumLength(200).WithMessage("Title cannot exceed 200 characters");

        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("Content is required");

        RuleFor(x => x.Slug)
            .NotEmpty().WithMessage("Slug is required")
            .Matches("^[a-z0-9]+(?:-[a-z0-9]+)*$").WithMessage("Slug must be URL-friendly");

        RuleFor(x => x.CategoryId)
            .GreaterThan(0).WithMessage("Category ID must be greater than 0");

        RuleFor(x => x.AuthorId)
            .NotEmpty().WithMessage("Author ID is required");

        RuleFor(x => x.TagIds)
            .NotNull().WithMessage("Tag IDs cannot be null");
    }
} 