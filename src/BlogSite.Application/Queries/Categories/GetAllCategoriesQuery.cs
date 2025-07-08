using MediatR;
using BlogSite.Application.DTOs;

namespace BlogSite.Application.Queries.Categories;

public record GetAllCategoriesQuery() : IRequest<IEnumerable<CategoryDto>>;