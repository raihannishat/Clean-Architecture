namespace BlogApp.API.Application.Features.Blog.Validators;

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