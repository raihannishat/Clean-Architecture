using FluentValidation;
using AutoRegister;

namespace BlogApp.API.Application.Features.Blog.Queries;

[Register(ServiceLifetime.Scoped)]
public class GetTagsQueryValidator : AbstractValidator<GetTagsQuery>
{
    public GetTagsQueryValidator()
    {
        // No validation rules needed for this query
    }
} 