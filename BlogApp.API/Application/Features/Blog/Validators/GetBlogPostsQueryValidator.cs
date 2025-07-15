using FluentValidation;
using AutoRegister;

namespace BlogApp.API.Application.Features.Blog.Queries;

[Register(ServiceLifetime.Scoped)]
public class GetBlogPostsQueryValidator : AbstractValidator<GetBlogPostsQuery>
{
    public GetBlogPostsQueryValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThan(0).WithMessage("Page must be greater than 0");

        RuleFor(x => x.PageSize)
            .GreaterThan(0).WithMessage("Page size must be greater than 0")
            .LessThanOrEqualTo(100).WithMessage("Page size cannot exceed 100");

        RuleFor(x => x.Category)
            .MaximumLength(100).WithMessage("Category name cannot exceed 100 characters");

        RuleFor(x => x.Tag)
            .MaximumLength(100).WithMessage("Tag name cannot exceed 100 characters");

        RuleFor(x => x.SearchTerm)
            .MaximumLength(200).WithMessage("Search term cannot exceed 200 characters");
    }
} 