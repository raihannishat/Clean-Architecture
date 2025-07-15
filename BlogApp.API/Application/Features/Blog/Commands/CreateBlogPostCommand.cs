using BlogApp.API.Application.CQRS;
using BlogApp.API.Application.Common;
using BlogApp.API.Core.Entities;
using BlogApp.API.Infrastructure.Persistence.UnitOfWork.Interfaces;
using AutoRegister;
using BlogApp.API.Application.Features.Blog.DTOs;
using AutoMapper;

namespace BlogApp.API.Application.Features.Blog.Commands;

public record CreateBlogPostCommand(
    string Title,
    string Content,
    string Slug,
    int CategoryId,
    List<int> TagIds,
    string AuthorId
) : ICommand<BaseResponse<BlogPostDTO>>;

[Register(ServiceLifetime.Scoped)]
public class CreateBlogPostCommandHandler : ICommandHandler<CreateBlogPostCommand, BaseResponse<BlogPostDTO>>
{
    private readonly ICommandUnitOfWork _unitOfWork;
    private readonly IOutboxService _outboxService;
    private readonly IMapper _mapper;

    public CreateBlogPostCommandHandler(IUnitOfWorkFactory unitOfWorkFactory, IOutboxService outboxService, IMapper mapper)
    {
        _unitOfWork = unitOfWorkFactory.CreateCommandUnitOfWork();
        _outboxService = outboxService;
        _mapper = mapper;
    }

    public async Task<BaseResponse<BlogPostDTO>> HandleAsync(CreateBlogPostCommand command, CancellationToken cancellationToken = default)
    {
        var category = await _unitOfWork.Repository<Category>().GetByIdAsync(command.CategoryId);
        if (category == null)
        {
            return BaseResponse<BlogPostDTO>.NotFound($"Category with ID {command.CategoryId} not found");
        }

        var tags = new List<Tag>();
        foreach (var tagId in command.TagIds)
        {
            var tag = await _unitOfWork.Repository<Tag>().GetByIdAsync(tagId);
            if (tag == null)
            {
                return BaseResponse<BlogPostDTO>.NotFound($"Tag with ID {tagId} not found");
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
        await _outboxService.AddAsync(nameof(CreateBlogPostCommand), command, cancellationToken);
        var blogPostDto = _mapper.Map<BlogPostDTO>(blogPost);
        return BaseResponse<BlogPostDTO>.Success(blogPostDto, "Blog post created successfully");
    }
} 