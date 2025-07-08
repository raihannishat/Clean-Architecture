using MediatR;
using BlogSite.Application.DTOs;

namespace BlogSite.Application.Queries.Authors;

public record GetAuthorByIdQuery(Guid Id) : IRequest<AuthorDto?>;