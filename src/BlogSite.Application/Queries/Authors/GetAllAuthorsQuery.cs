using MediatR;
using BlogSite.Application.DTOs;

namespace BlogSite.Application.Queries.Authors;

public record GetAllAuthorsQuery() : IRequest<IEnumerable<AuthorDto>>;