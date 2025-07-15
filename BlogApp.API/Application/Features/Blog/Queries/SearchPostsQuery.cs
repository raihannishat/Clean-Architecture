using BlogApp.API.Application.CQRS;
using BlogApp.API.Application.Common;
using BlogApp.API.Core.Entities;
using BlogApp.API.Infrastructure.Persistence.UnitOfWork.Interfaces;
using AutoRegister;
using BlogApp.API.Application.Features.Blog.DTOs;
using AutoMapper;

namespace BlogApp.API.Application.Features.Blog.Queries;

public record SearchPostsQuery(
    string SearchTerm,
    int Page = 1,
    int PageSize = 10,
    bool IncludeUnpublished = false
) : IQuery<BaseResponse<List<BlogPostDTO>>>;

[Register(ServiceLifetime.Scoped)]
public class SearchPostsQueryHandler : IQueryHandler<SearchPostsQuery, BaseResponse<List<BlogPostDTO>>>
{
    private readonly IQueryUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public SearchPostsQueryHandler(IUnitOfWorkFactory unitOfWorkFactory, IMapper mapper)
    {
        _unitOfWork = unitOfWorkFactory.CreateQueryUnitOfWork();
        _mapper = mapper;
    }

    public async Task<BaseResponse<List<BlogPostDTO>>> HandleAsync(SearchPostsQuery query, CancellationToken cancellationToken = default)
    {
        var posts = await _unitOfWork.Repository<BlogPost>().GetAllAsync();
        var filteredPosts = posts.Where(p => 
            (query.IncludeUnpublished || p.IsPublished) &&
            (p.Title.Contains(query.SearchTerm, StringComparison.OrdinalIgnoreCase) || 
             p.Content.Contains(query.SearchTerm, StringComparison.OrdinalIgnoreCase))
        ).ToList();

        var pagedPosts = filteredPosts
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToList();

        var postDtos = pagedPosts.Select(p => _mapper.Map<BlogPostDTO>(p)).ToList();
        return BaseResponse<List<BlogPostDTO>>.Success(postDtos, "Search completed successfully");
    }
} 