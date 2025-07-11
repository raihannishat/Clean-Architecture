using BlogApp.API.Application.CQRS;
using BlogApp.API.Application.Common;
using BlogApp.API.Core.Entities;
using BlogApp.API.Infrastructure.Persistence.UnitOfWork.Interfaces;
using FluentValidation;
using AutoRegister;

namespace BlogApp.API.Application.Features.Blog.Queries;

public class GetTagsQuery : IQuery<BaseResponse<List<Tag>>>
{
    public bool IncludeInactive { get; set; } = false;
}

[Register(ServiceLifetime.Scoped)]
public class GetTagsQueryHandler : IQueryHandler<GetTagsQuery, BaseResponse<List<Tag>>>
{
    private readonly IQueryUnitOfWork _unitOfWork;

    public GetTagsQueryHandler(IUnitOfWorkFactory unitOfWorkFactory)
    {
        _unitOfWork = unitOfWorkFactory.CreateQueryUnitOfWork();
    }

    public async Task<BaseResponse<List<Tag>>> HandleAsync(GetTagsQuery query, CancellationToken cancellationToken = default)
    {
        var tags = await _unitOfWork.Repository<Tag>().GetAllAsync();
        var filteredTags = tags.Where(t => query.IncludeInactive || t.IsActive).ToList();

        return BaseResponse<List<Tag>>.Success(filteredTags, "Tags retrieved successfully");
    }
}

[Register(ServiceLifetime.Scoped)]
public class GetTagsQueryValidator : AbstractValidator<GetTagsQuery>
{
    public GetTagsQueryValidator()
    {
        // No validation rules needed for this query
    }
} 