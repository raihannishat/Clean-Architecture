using BlogApp.API.Application.CQRS;
using BlogApp.API.Application.Common;
using BlogApp.API.Core.Entities;
using BlogApp.API.Infrastructure.Persistence.UnitOfWork.Interfaces;
using AutoRegister;
using BlogApp.API.Application.Features.Blog.DTOs;
using AutoMapper;

namespace BlogApp.API.Application.Features.Blog.Queries;

public record GetTagsQuery(
    bool IncludeInactive = false
) : IQuery<BaseResponse<List<TagDTO>>>;

[Register(ServiceLifetime.Scoped)]
public class GetTagsQueryHandler : IQueryHandler<GetTagsQuery, BaseResponse<List<TagDTO>>>
{
    private readonly IQueryUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetTagsQueryHandler(IUnitOfWorkFactory unitOfWorkFactory, IMapper mapper)
    {
        _unitOfWork = unitOfWorkFactory.CreateQueryUnitOfWork();
        _mapper = mapper;
    }

    public async Task<BaseResponse<List<TagDTO>>> HandleAsync(GetTagsQuery query, CancellationToken cancellationToken = default)
    {
        var tags = await _unitOfWork.Repository<Tag>().GetAllAsync();
        var filteredTags = tags.Where(t => query.IncludeInactive || t.IsActive).ToList();
        var tagDtos = filteredTags.Select(t => _mapper.Map<TagDTO>(t)).ToList();
        return BaseResponse<List<TagDTO>>.Success(tagDtos, "Tags retrieved successfully");
    }
} 