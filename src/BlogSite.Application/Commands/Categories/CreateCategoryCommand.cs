using MediatR;
using BlogSite.Application.DTOs;

namespace BlogSite.Application.Commands.Categories;

public record CreateCategoryCommand(
    string Name,
    string? Description = null
) : IRequest<CategoryDto>;