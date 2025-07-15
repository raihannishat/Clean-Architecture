using BlogApp.API.Application.CQRS;
using BlogApp.API.Application.Common;
using BlogApp.API.Core.Entities;
using BlogApp.API.Infrastructure.Persistence.UnitOfWork.Interfaces;
using AutoRegister;
using BlogApp.API.Application.Features.Blog.DTOs;
using AutoMapper;

namespace BlogApp.API.Application.Features.Blog.Queries;

public record GetBlogPostsQuery(
    int Page = 1,
    int PageSize = 10,
    string? Category = null,
    string? Tag = null,
    string? SearchTerm = null,
    bool IncludeUnpublished = false
) : IQuery<BaseResponse<List<BlogPostDTO>>>;

[Register(ServiceLifetime.Scoped)]
public class GetBlogPostsQueryHandler : IQueryHandler<GetBlogPostsQuery, BaseResponse<List<BlogPostDTO>>>
{
    private readonly IQueryUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetBlogPostsQueryHandler(IUnitOfWorkFactory unitOfWorkFactory, IMapper mapper)
    {
        _unitOfWork = unitOfWorkFactory.CreateQueryUnitOfWork();
        _mapper = mapper;
    }

    public async Task<BaseResponse<List<BlogPostDTO>>> HandleAsync(GetBlogPostsQuery query, CancellationToken cancellationToken = default)
    {
        var posts = await _unitOfWork.Repository<BlogPost>().GetAllAsync();
        var postDtos = posts.Select(p => _mapper.Map<BlogPostDTO>(p)).ToList();
        return BaseResponse<List<BlogPostDTO>>.Success(postDtos, "Blog posts retrieved successfully");
    }
} 