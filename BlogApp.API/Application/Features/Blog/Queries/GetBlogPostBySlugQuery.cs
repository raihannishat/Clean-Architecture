using BlogApp.API.Application.CQRS;
using BlogApp.API.Application.Common;
using BlogApp.API.Core.Entities;
using BlogApp.API.Infrastructure.Persistence.UnitOfWork.Interfaces;
using AutoRegister;
using BlogApp.API.Application.Features.Blog.DTOs;
using AutoMapper;

namespace BlogApp.API.Application.Features.Blog.Queries;

public record GetBlogPostBySlugQuery(
    string Slug,
    bool IncludeUnpublished = false
) : IQuery<BaseResponse<BlogPostDTO>>;

[Register(ServiceLifetime.Scoped)]
public class GetBlogPostBySlugQueryHandler : IQueryHandler<GetBlogPostBySlugQuery, BaseResponse<BlogPostDTO>>
{
    private readonly IQueryUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetBlogPostBySlugQueryHandler(IUnitOfWorkFactory unitOfWorkFactory, IMapper mapper)
    {
        _unitOfWork = unitOfWorkFactory.CreateQueryUnitOfWork();
        _mapper = mapper;
    }

    public async Task<BaseResponse<BlogPostDTO>> HandleAsync(GetBlogPostBySlugQuery query, CancellationToken cancellationToken = default)
    {
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