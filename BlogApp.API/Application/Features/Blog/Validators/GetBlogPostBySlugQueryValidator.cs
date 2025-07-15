using FluentValidation;
using AutoRegister;

namespace BlogApp.API.Application.Features.Blog.Queries;

[Register(ServiceLifetime.Scoped)]
public class GetBlogPostBySlugQueryValidator : AbstractValidator<GetBlogPostBySlugQuery>
{
    public GetBlogPostBySlugQueryValidator()
    {
        RuleFor(x => x.Slug)
            .NotEmpty().WithMessage("Slug is required")
            .Matches("^[a-z0-9]+(?:-[a-z0-9]+)*$").WithMessage("Slug must be URL-friendly");
    }
} 