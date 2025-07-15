using FluentValidation;
using AutoRegister;

namespace BlogApp.API.Application.Features.Blog.Queries;

[Register(ServiceLifetime.Scoped)]
public class GetCategoriesQueryValidator : AbstractValidator<GetCategoriesQuery>
{
    public GetCategoriesQueryValidator()
    {
        // No validation rules needed for this query
    }
} 