using MediatR;
using BlogSite.Application.DTOs;

namespace BlogSite.Application.Queries.Authors;

public record GetAuthorByEmailQuery(string Email) : IRequest<AuthorDto?>;