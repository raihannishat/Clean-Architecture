

namespace BlogApp.API.Application.Features.Blog.Validators;

[Register(ServiceLifetime.Scoped)]
public class GetTagsQueryValidator : AbstractValidator<GetTagsQuery>
{
    public GetTagsQueryValidator()
    {
        // No validation rules needed for this query
    }
} 